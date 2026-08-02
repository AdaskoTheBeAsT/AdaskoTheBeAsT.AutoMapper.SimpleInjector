using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class DestinationFactoryAndConditionTests
    : IDisposable
{
    private readonly Container _container;

    public DestinationFactoryAndConditionTests(ITestOutputHelper output)
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
        _container.AddAutoMapper(config =>
            config
                .WithMapperAssemblyMarkerTypes(typeof(Source))
                .WithMapperConfigurationExpressionAction((_, expression) =>
                    expression.CreateMap<Source4, Dest4>()
                        .ConstructUsing<DependencyDestinationFactory>()
                        .ForMember(d => d.ConstructedValue, opt => opt.Ignore())
                        .ForMember(d => d.ConditionValue, opt => opt.Condition<DependencyCondition>())
                        .ForMember(d => d.PreConditionValue, opt => opt.PreCondition<DependencyPreCondition>())));
    }

    [Fact]
    public void ShouldConstructDestinationWithDependency()
    {
        var mapper = _container.GetInstance<IMapper>();

        var dest = mapper.Map<Source4, Dest4>(new Source4());

        dest.ConstructedValue.Should().Be(6);
    }

    [Fact]
    public void ShouldMapMemberWhenConditionWithDependencyIsMet()
    {
        var mapper = _container.GetInstance<IMapper>();

        var dest = mapper.Map<Source4, Dest4>(new Source4 { ConditionValue = 100 });

        dest.ConditionValue.Should().Be(100);
    }

    [Fact]
    public void ShouldNotMapMemberWhenConditionWithDependencyIsNotMet()
    {
        var mapper = _container.GetInstance<IMapper>();

        var dest = mapper.Map<Source4, Dest4>(new Source4 { ConditionValue = 1 });

        dest.ConditionValue.Should().Be(0);
    }

    [Fact]
    public void ShouldMapMemberWhenPreConditionWithDependencyIsMet()
    {
        var mapper = _container.GetInstance<IMapper>();

        var dest = mapper.Map<Source4, Dest4>(new Source4 { PreConditionValue = 100 });

        dest.PreConditionValue.Should().Be(100);
    }

    [Fact]
    public void ShouldNotMapMemberWhenPreConditionWithDependencyIsNotMet()
    {
        var mapper = _container.GetInstance<IMapper>();

        var dest = mapper.Map<Source4, Dest4>(new Source4 { PreConditionValue = 1 });

        dest.PreConditionValue.Should().Be(0);
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
