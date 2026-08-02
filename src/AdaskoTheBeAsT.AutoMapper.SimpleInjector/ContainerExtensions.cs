using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector;

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
/// <summary>
/// Extensions to scan for AutoMapper classes and register the configuration, mapping, and extensions with SimpleInjector:
/// <list type="bullet">
/// <item> Finds <see cref="Profile"/> classes and initializes a new <see cref="MapperConfiguration" />,</item>
/// <item> Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/>,
/// <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" />, <see cref="IValueConverter{TSourceMember,TDestinationMember}"/>,
/// <see cref="IDestinationFactory{TSource,TDestination}"/>, <see cref="ICondition{TSource,TDestination,TMember}"/>,
/// <see cref="IPreCondition{TSource,TDestination}"/> and <see cref="IMappingAction{TSource,TDestination}"/>
/// implementations and registers them as <see cref="Lifestyle.Transient"/>, </item>
/// <item> Registers <see cref="IConfigurationProvider"/> as <see cref="Lifestyle.Singleton"/>, and</item>
/// <item> Registers <see cref="IMapper"/> as a configurable <see cref="Lifestyle"/> (default is <see cref="Lifestyle.Singleton"/>)</item>
/// </list>
/// After calling AddAutoMapper you can resolve an <see cref="IMapper" /> instance from a scoped service provider, or as a dependency
/// To use <see cref="AutoMapper.QueryableExtensions.Extensions.ProjectTo{TDestination}(IQueryable,IConfigurationProvider, System.Linq.Expressions.Expression{System.Func{TDestination, object}}[])" />
/// you can resolve the <see cref="IConfigurationProvider"/> instance directly for from an <see cref="IMapper" /> instance.
/// </summary>
public static class ContainerExtensions
{
    /// <summary>
    /// Open generic AutoMapper interfaces whose implementations are auto registered.
    /// Kept in sync with AmTypes in AutoMapper's own ServiceCollectionExtensions.
    /// </summary>
    private static readonly Type[] AutoMapperOpenTypes =
    [
        typeof(IValueResolver<,,>),
        typeof(IMemberValueResolver<,,,>),
        typeof(ITypeConverter<,>),
        typeof(IValueConverter<,>),
        typeof(IDestinationFactory<,>),
        typeof(ICondition<,,>),
        typeof(IPreCondition<,>),
        typeof(IMappingAction<,>),
    ];

    /// <summary>
    /// Add AutoMapper to SimpleInjector with assemblies to scan.
    /// </summary>
    /// <param name="container"><see cref="Container"/>.</param>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <returns><see cref="Container"/>.</returns>
    public static Container AddAutoMapper(
        this Container container,
        params Assembly[] assemblies)
    {
        return AddAutoMapper(container, configuration => configuration.WithAssembliesToScan(assemblies));
    }

    /// <summary>
    /// Add AutoMapper to SimpleInjector with assemblies to scan.
    /// </summary>
    /// <param name="container"><see cref="Container"/>.</param>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <returns><see cref="Container"/>.</returns>
    public static Container AddAutoMapper(
        this Container container,
        IEnumerable<Assembly> assemblies)
    {
        return AddAutoMapper(container, configuration => configuration.WithAssembliesToScan(assemblies));
    }

    /// <summary>
    /// Add AutoMapper to SimpleInjector with types from the assemblies that contain the specified types.
    /// </summary>
    /// <param name="container"><see cref="Container"/>.</param>
    /// <param name="mapperAssemblyMarkerTypes">Types used to mark assemblies to scan.</param>
    /// <returns><see cref="Container"/>.</returns>
    public static Container AddAutoMapper(
        this Container container,
        params Type[] mapperAssemblyMarkerTypes)
    {
        return AddAutoMapper(container, configuration => configuration.WithMapperAssemblyMarkerTypes(mapperAssemblyMarkerTypes));
    }

    /// <summary>
    /// Add AutoMapper to SimpleInjector with types from the assemblies that contain the specified types.
    /// </summary>
    /// <param name="container"><see cref="Container"/>.</param>
    /// <param name="mapperAssemblyMarkerTypes">Types used to mark assemblies to scan.</param>
    /// <returns><see cref="Container"/>.</returns>
    public static Container AddAutoMapper(
        this Container container,
        IEnumerable<Type> mapperAssemblyMarkerTypes)
    {
        return AddAutoMapper(container, configuration => configuration.WithMapperAssemblyMarkerTypes(mapperAssemblyMarkerTypes));
    }

    /// <summary>
    /// Add AutoMapper to SimpleInjector with configuration action.
    /// </summary>
    /// <param name="container"><see cref="Container"/>.</param>
    /// <param name="configuration"><see cref="AutoMapperSimpleInjectorConfiguration"/> instance.</param>
    /// <returns><see cref="Container"/>.</returns>
    public static Container AddAutoMapper(
        this Container container,
        Action<AutoMapperSimpleInjectorConfiguration>? configuration)
    {
        ArgumentNullException.ThrowIfNull(container);

        var serviceConfig = new AutoMapperSimpleInjectorConfiguration();
        configuration?.Invoke(serviceConfig);

        var uniqueAssemblies = serviceConfig.AssembliesToScan.Distinct().ToArray();

        // A single class can implement several AutoMapper interfaces, so concrete
        // self registrations have to be deduplicated across all open types.
        var selfRegistered = new HashSet<Type>();
        foreach (var openType in AutoMapperOpenTypes)
        {
            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, openType, selfRegistered);
        }

        var customMapperInstance = serviceConfig.MapperInstanceCreator();

        if (customMapperInstance != null)
        {
            container.Register(
                () => customMapperInstance,
                serviceConfig.Lifestyle);
        }
        else
        {
            container.Register<IConfigurationProvider>(
                () =>
                {
                    var loggerFactory = container.GetInstance<ILoggerFactory>();
                    return new MapperConfiguration(
                        cfg => container.ConfigAction(cfg, serviceConfig),
                        loggerFactory);
                },
                Lifestyle.Singleton);

            if (serviceConfig.MapperImplementationType == typeof(Mapper))
            {
                container.Register(
                    () => container.GetInstance<IConfigurationProvider>().CreateMapper(),
                    serviceConfig.Lifestyle);
            }
            else
            {
                container.Register(typeof(IMapper), serviceConfig.MapperImplementationType, serviceConfig.Lifestyle);
            }
        }

        return container;
    }

    private static void ConfigAction(
        this Container c,
        IMapperConfigurationExpression cfg,
        AutoMapperSimpleInjectorConfiguration serviceCfg)
    {
        serviceCfg.MapperConfigurationExpressionAction.Invoke(
            c,
            cfg);
        cfg.ConstructServicesUsing(c.GetInstance);
        cfg.AddMaps(serviceCfg.AssembliesToScan);

        if (!string.IsNullOrEmpty(serviceCfg.LicenseKey))
        {
            cfg.LicenseKey = serviceCfg.LicenseKey;
        }
    }

    private static void RegisterIncludingGenericTypeDefinitions(
        this Container container,
        Assembly[] uniqueAssemblies,
        Type processorType,
        HashSet<Type> selfRegistered)
    {
        var implementingTypes = container.GetTypesToRegister(
            processorType,
            uniqueAssemblies,
            new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            }).ToList();

        container.Collection.Register(processorType, implementingTypes);

        var notYetRegistered = implementingTypes.Where(t => !selfRegistered.Contains(t)).ToList();
        foreach (var implementingType in notYetRegistered)
        {
            container.Register(implementingType, implementingType, Lifestyle.Transient);
            _ = selfRegistered.Add(implementingType);
        }
    }
}
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
