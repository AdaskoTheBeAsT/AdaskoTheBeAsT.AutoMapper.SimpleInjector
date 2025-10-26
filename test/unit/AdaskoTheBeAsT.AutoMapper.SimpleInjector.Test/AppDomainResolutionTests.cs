using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AutoMapper.Internal;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class AppDomainResolutionTests
    : IDisposable
{
    private readonly Container _container;

    public AppDomainResolutionTests(ITestOutputHelper output)
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
        _container.AddAutoMapper(typeof(AppDomainResolutionTests));
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
        globalConfiguration!.GetAllTypeMaps().Should().HaveCount(3);
    }

    [Fact]
    public void ShouldResolveMapper()
    {
        _container.GetInstance<IMapper>().Should().NotBeNull();
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
