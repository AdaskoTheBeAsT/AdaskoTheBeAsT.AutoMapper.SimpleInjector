using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using SimpleInjector;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector;

/// <summary>
/// AutoMapper SimpleInjector configuration file.
/// </summary>
public class AutoMapperSimpleInjectorConfiguration
{
    public AutoMapperSimpleInjectorConfiguration()
    {
        MapperImplementationType = typeof(Mapper);
        Lifestyle = Lifestyle.Singleton;
        AssembliesToScan = [];
        MapperConfigurationExpressionAction = (
            container,
            expression) =>
        {
            // ReSharper disable once AssignmentIsFullyDiscarded
            _ = container;

            // ReSharper disable once AssignmentIsFullyDiscarded
            _ = expression;
        };
        MapperInstanceCreator = () => null;
    }

    /// <summary>
    /// Custom implementation of <see cref="IMapper"/>.
    /// Default is <see cref="Mapper"/>.
    /// </summary>
    public Type MapperImplementationType { get; private set; }

    /// <summary>
    /// Custom instance creator of <see cref="IMapper"/>.
    /// Can be used for mocking AutoMapper.
    /// If not null then it takes precedence over MapperImplementationType.
    /// MapperConfigurationExpressionAction is not invoked then.
    /// </summary>
    public Func<IMapper?> MapperInstanceCreator { get; private set; }

    /// <summary>
    /// Lifestyle in which <see cref="IMapper"/> implementation will be registered.
    /// Default is <see cref="Lifestyle"/>.Singleton.
    /// </summary>
    public Lifestyle Lifestyle { get; private set; }

    /// <summary>
    /// Assemblies which will be scanned for
    /// auto registering types implementing AutoMapper interfaces.
    /// </summary>
    public IEnumerable<Assembly> AssembliesToScan { get; private set; }

    /// <summary>
    /// Configuration action for MapperConfigurationExpression.
    /// </summary>
    public Action<Container, IMapperConfigurationExpression> MapperConfigurationExpressionAction { get; private set; }

    /// <summary>
    /// Register custom implementation of <see cref="IMapper"/> type
    /// instead of default one <see cref="Mapper"/>.
    /// </summary>
    /// <typeparam name="TMapper">Custom <see cref="IMapper"/> implementation.</typeparam>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with custom <see cref="IMapper"/> implementation.</returns>
    public AutoMapperSimpleInjectorConfiguration Using<TMapper>()
        where TMapper : IMapper
    {
        MapperImplementationType = typeof(TMapper);
        return this;
    }

    /// <summary>
    /// Register custom IMapper instance creator
    /// instead of default one <see cref="Mapper"/>.
    /// MapperConfigurationExpressionAction is not invoked when instance is used.
    /// </summary>
    /// <param name="instanceCreator">Custom <see cref="IMapper"/> instance creator function.</param>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with custom <see cref="IMapper"/> instance creator function.</returns>
    public AutoMapperSimpleInjectorConfiguration Using(Func<IMapper> instanceCreator)
    {
        MapperInstanceCreator =
            instanceCreator
            ?? throw new ArgumentNullException(nameof(instanceCreator));
        return this;
    }

    /// <summary>
    /// Set lifestyle of custom or default
    /// <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Singleton.
    /// </summary>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Singleton.</returns>
    public AutoMapperSimpleInjectorConfiguration AsSingleton()
    {
        Lifestyle = Lifestyle.Singleton;
        return this;
    }

    /// <summary>
    /// Set lifestyle of custom or default
    /// <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Scoped.
    /// </summary>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Scoped.</returns>
    public AutoMapperSimpleInjectorConfiguration AsScoped()
    {
        Lifestyle = Lifestyle.Scoped;
        return this;
    }

    /// <summary>
    /// Set lifestyle of custom or default
    /// <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Transient.
    /// </summary>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with <see cref="IMapper"/> implementation to <see cref="Lifestyle"/>.Transient.</returns>
    public AutoMapperSimpleInjectorConfiguration AsTransient()
    {
        Lifestyle = Lifestyle.Transient;
        return this;
    }

    /// <summary>
    /// Setup assemblies which will be scanned for
    /// auto registering types implementing AutoMapper interfaces.
    /// </summary>
    /// <param name="assembliesToScan">Assemblies which will be scanned for
    /// auto registering types.</param>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with assemblies to scan configured.</returns>
    public AutoMapperSimpleInjectorConfiguration WithAssembliesToScan(IEnumerable<Assembly> assembliesToScan)
    {
        AssembliesToScan = assembliesToScan;
        return this;
    }

    /// <summary>
    /// Setup assemblies which will be scanned for
    /// auto registering types implementing AutoMapper interfaces
    /// by types from given assemblies (marker types).
    /// </summary>
    /// <param name="mapperAssemblyMarkerTypes">Types from assemblies which will be scanned for
    /// auto registering types.</param>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with assemblies to scan configured.</returns>
    public AutoMapperSimpleInjectorConfiguration WithMapperAssemblyMarkerTypes(params Type[] mapperAssemblyMarkerTypes)
    {
        AssembliesToScan = mapperAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly);
        return this;
    }

    /// <summary>
    /// Setup assemblies which will be scanned for
    /// auto registering types implementing AutoMapper interfaces
    /// by types from given assemblies (marker types).
    /// </summary>
    /// <param name="mapperAssemblyMarkerTypes">Types from assemblies which will be scanned for
    /// auto registering types.</param>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with assemblies to scan configured.</returns>
    public AutoMapperSimpleInjectorConfiguration WithMapperAssemblyMarkerTypes(IEnumerable<Type> mapperAssemblyMarkerTypes)
    {
        AssembliesToScan = mapperAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly);
        return this;
    }

    /// <summary>
    /// Setup configuration action for <see cref="IMapperConfigurationExpression"/> instance.
    /// </summary>
    /// <param name="mapperConfigurationExpressionAction"><see cref="IMapperConfigurationExpression"/> instance.</param>
    /// <returns><see cref="AutoMapperSimpleInjectorConfiguration"/>
    /// with configuration action for <see cref="IMapperConfigurationExpression"/> instance configured.</returns>
    public AutoMapperSimpleInjectorConfiguration WithMapperConfigurationExpressionAction(
        Action<Container, IMapperConfigurationExpression> mapperConfigurationExpressionAction)
    {
        MapperConfigurationExpressionAction = mapperConfigurationExpressionAction
                                              ?? throw new ArgumentNullException(nameof(mapperConfigurationExpressionAction));
        return this;
    }
}
