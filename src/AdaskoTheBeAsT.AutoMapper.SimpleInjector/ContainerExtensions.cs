using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using SimpleInjector;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Extensions to scan for AutoMapper classes and register the configuration, mapping, and extensions with SimpleInjector:
    /// <list type="bullet">
    /// <item> Finds <see cref="Profile"/> classes and initializes a new <see cref="MapperConfiguration" />,</item>
    /// <item> Scans for <see cref="ITypeConverter{TSource,TDestination}"/>, <see cref="IValueResolver{TSource,TDestination,TDestMember}"/>,
    /// <see cref="IMemberValueResolver{TSource,TDestination,TSourceMember,TDestMember}" /> and <see cref="IMappingAction{TSource,TDestination}"/>
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
        /// Add AutoMapper to SimpleInjector with types from the assemblies that contain the specified types..
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
        /// Add AutoMapper to SimpleInjector with types from the assemblies that contain the specified types..
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
            var serviceConfig = new AutoMapperSimpleInjectorConfiguration();
            configuration?.Invoke(serviceConfig);

            var uniqueAssemblies = serviceConfig.AssembliesToScan.Distinct().ToArray();

            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, typeof(IValueResolver<,,>));
            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, typeof(IMemberValueResolver<,,,>));
            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, typeof(ITypeConverter<,>));
            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, typeof(IValueConverter<,>));
            container.RegisterIncludingGenericTypeDefinitions(uniqueAssemblies, typeof(IMappingAction<,>));

            void ConfigAction(
                Container c,
                IMapperConfigurationExpression cfg,
                AutoMapperSimpleInjectorConfiguration serviceCfg)
            {
                serviceCfg.MapperConfigurationExpressionAction?.Invoke(
                    c,
                    cfg);
                cfg.ConstructServicesUsing(c.GetInstance);
                cfg.AddMaps(serviceCfg.AssembliesToScan);
            }

            container.Register<IConfigurationProvider>(
                () =>
                    new MapperConfiguration(
                        cfg => ConfigAction(container, cfg, serviceConfig)),
                Lifestyle.Singleton);

            var customMapperInstance = serviceConfig.MapperInstanceCreator();

            if (customMapperInstance != null)
            {
                container.Register(
                    () => customMapperInstance,
                    serviceConfig.Lifestyle);
            }
            else if (serviceConfig.MapperImplementationType == typeof(Mapper))
            {
                container.Register(
                    () => container.GetInstance<IConfigurationProvider>().CreateMapper(),
                    serviceConfig.Lifestyle);
            }
            else
            {
                container.Register(typeof(IMapper), serviceConfig.MapperImplementationType, serviceConfig.Lifestyle);
            }

            return container;
        }

        internal static void RegisterIncludingGenericTypeDefinitions(
            this Container container,
            Assembly[] uniqueAssemblies,
            Type processorType)
        {
            var implementingTypes = container.GetTypesToRegister(
                processorType,
                uniqueAssemblies,
                new TypesToRegisterOptions
                {
                    IncludeGenericTypeDefinitions = true,
                    IncludeComposites = false,
                });

            container.Collection.Register(processorType, implementingTypes);
        }
    }
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
}
