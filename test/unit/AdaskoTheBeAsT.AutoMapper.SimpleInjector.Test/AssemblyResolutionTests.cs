using System;
using System.Collections.Generic;
using System.Reflection;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AutoMapper.Internal;
using AwesomeAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class AssemblyResolutionTests
    : IDisposable
{
    private readonly Container _container;

    public AssemblyResolutionTests()
    {
        _container = new Container();
        _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
        _container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
    }

    [Fact]
    public void ShouldResolveConfigurationUsingAssemblyParams()
    {
        _container.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
        _container.GetInstance<IConfigurationProvider>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldConfigureProfilesUsingAssemblyParams()
    {
        _container.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
        var configurationProvider = _container.GetInstance<IConfigurationProvider>();
        configurationProvider.Should().NotBeNull();
        var globalConfiguration = configurationProvider as IGlobalConfiguration;
        globalConfiguration.Should().NotBeNull();
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
    }

    [Fact]
    public void ShouldResolveMapperUsingAssemblyParams()
    {
        _container.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveConfigurationUsingAssemblyIEnumerable()
    {
        _container.AddAutoMapper(new List<Assembly> { typeof(Source).GetTypeInfo().Assembly });
        _container.GetInstance<IConfigurationProvider>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldConfigureProfilesUsingAssemblyIEnumerable()
    {
        _container.AddAutoMapper(new List<Assembly> { typeof(Source).GetTypeInfo().Assembly });
        var configurationProvider = _container.GetInstance<IConfigurationProvider>();
        configurationProvider.Should().NotBeNull();
        var globalConfiguration = configurationProvider as IGlobalConfiguration;
        globalConfiguration.Should().NotBeNull();
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
    }

    [Fact]
    public void ShouldResolveMapperUsingAssemblyIEnumerable()
    {
        _container.AddAutoMapper(new List<Assembly> { typeof(Source).GetTypeInfo().Assembly });
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveConfigurationUsingMapperAssemblyMarkerTypeParams()
    {
        _container.AddAutoMapper(typeof(Source));
        _container.GetInstance<IConfigurationProvider>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldConfigureProfilesUsingMapperAssemblyMarkerTypeParams()
    {
        _container.AddAutoMapper(typeof(Source));
        var configurationProvider = _container.GetInstance<IConfigurationProvider>();
        configurationProvider.Should().NotBeNull();
        var globalConfiguration = configurationProvider as IGlobalConfiguration;
        globalConfiguration.Should().NotBeNull();
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
    }

    [Fact]
    public void ShouldResolveMapperUsingMapperAssemblyMarkerTypeParams()
    {
        _container.AddAutoMapper(typeof(Source));
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveConfigurationUsingMapperAssemblyMarkerTypeIEnumerable()
    {
        _container.AddAutoMapper(new List<Type> { typeof(Source) });
        _container.GetInstance<IConfigurationProvider>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldConfigureProfilesUsingMapperAssemblyMarkerTypeIEnumerable()
    {
        _container.AddAutoMapper(new List<Type> { typeof(Source) });
        var configurationProvider = _container.GetInstance<IConfigurationProvider>();
        configurationProvider.Should().NotBeNull();
        var globalConfiguration = configurationProvider as IGlobalConfiguration;
        globalConfiguration.Should().NotBeNull();
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
    }

    [Fact]
    public void ShouldResolveMapperUsingMapperAssemblyMarkerTypeIEnumerable()
    {
        _container.AddAutoMapper(new List<Type> { typeof(Source) });
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
