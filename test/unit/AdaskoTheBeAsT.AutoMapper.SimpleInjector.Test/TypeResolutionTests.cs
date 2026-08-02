using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AutoMapper.Internal;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class TypeResolutionTests
    : IDisposable
{
    private readonly Container _container;

    public TypeResolutionTests(ITestOutputHelper output)
    {
        _container = new Container();
        _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
        _container.RegisterInstance(output);

        // ILoggerFactory that writes to test output
        _container.RegisterSingleton<ILoggerFactory>(() =>
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
#pragma warning disable IDISP004
                builder.AddProvider(new XunitTestOutputLoggerProvider(output));
#pragma warning restore IDISP004
                builder.SetMinimumLevel(LogLevel.Trace);
            }));

        // Wire up ILogger<T> using the factory
        _container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        _container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
        _container.AddAutoMapper(typeof(Source));
    }

    [Fact]
    public void ShouldResolveConfiguration()
    {
        _container.GetInstance<IConfigurationProvider>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldConfigureProfiles()
    {
        var configurationProvider = _container.GetInstance<IConfigurationProvider>();
        configurationProvider.Should().NotBeNull();
        var globalConfiguration = configurationProvider as IGlobalConfiguration;
        globalConfiguration.Should().NotBeNull();
#pragma warning disable S8969
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
#pragma warning restore S8969
    }

    [Fact]
    public void ShouldResolveMapper()
    {
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveMappingAction()
    {
        _container.GetInstance<FooMappingAction>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveValueResolver()
    {
        _container.GetInstance<FooValueResolver>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveMemberValueResolver()
    {
        _container.GetInstance<FooMemberValueResolver>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveTypeConverter()
    {
        _container.GetInstance<FooTypeConverter>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveValueConverter()
    {
        _container.GetInstance<FooValueConverter>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveDestinationFactory()
    {
        _container.GetInstance<FooDestinationFactory>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolveCondition()
    {
        _container.GetInstance<FooCondition>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldResolvePreCondition()
    {
        _container.GetInstance<FooPreCondition>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldRegisterAllAutoMapperInterfacesAsCollections()
    {
        _container.GetAllInstances<IValueResolver<object, object, object>>().Should().ContainItemsAssignableTo<FooValueResolver>();
        _container.GetAllInstances<IMemberValueResolver<object, object, object, object>>().Should().ContainItemsAssignableTo<FooMemberValueResolver>();
        _container.GetAllInstances<ITypeConverter<object, object>>().Should().ContainItemsAssignableTo<FooTypeConverter>();
        _container.GetAllInstances<IValueConverter<int, int>>().Should().NotBeEmpty();
        _container.GetAllInstances<IDestinationFactory<object, object>>().Should().ContainItemsAssignableTo<FooDestinationFactory>();
        _container.GetAllInstances<ICondition<object, object, object>>().Should().ContainItemsAssignableTo<FooCondition>();
        _container.GetAllInstances<IPreCondition<object, object>>().Should().ContainItemsAssignableTo<FooPreCondition>();
        _container.GetAllInstances<IMappingAction<object, object>>().Should().ContainItemsAssignableTo<FooMappingAction>();
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
