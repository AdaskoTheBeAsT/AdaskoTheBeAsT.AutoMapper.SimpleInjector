using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class DependencyTests
    : IDisposable
{
    private readonly Container _container;

    public DependencyTests(ITestOutputHelper output)
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
        _container.AddAutoMapper(typeof(Source), typeof(Profile));
        _container.GetInstance<IConfigurationProvider>().AssertConfigurationIsValid();
    }

    [Fact]
    public void ShouldThrowExceptionWhenNullContainerPassed()
    {
        // Arrange
        const Container? container = null;
#pragma warning disable 8604

        // ReSharper disable once InvokeAsExtensionMethod
#pragma warning disable CC0026 // Call Extension Method As Extension
        Action action = () => ContainerExtensions.AddAutoMapper(container!, _ => { });
#pragma warning restore CC0026 // Call Extension Method As Extension
#pragma warning restore 8604

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldResolveWithDependency()
    {
        var mapper = _container.GetInstance<IMapper>();
        var dest = mapper.Map<Source2, Dest2>(new Source2());

        dest.ResolvedValue.Should().Be(5);
    }

    [Fact]
    public void ShouldConvertWithDependency()
    {
        var mapper = _container.GetInstance<IMapper>();
        var dest = mapper.Map<Source2, Dest2>(new Source2 { ConvertedValue = 5 });

        dest.ConvertedValue.Should().Be(10);
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
