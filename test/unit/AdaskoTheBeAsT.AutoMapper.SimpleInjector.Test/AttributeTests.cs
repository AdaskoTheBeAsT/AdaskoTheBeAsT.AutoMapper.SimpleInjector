using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

#pragma warning disable CA1707 // Identifiers should not contain underscores
public class AttributeTests(ITestOutputHelper output)
{
    [Fact]
    public void Should_not_register_static_instance_when_configured()
    {
        using var container = new Container();
        container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
        container.RegisterInstance(output);

        // ILoggerFactory that writes to test output
        container.RegisterSingleton<ILoggerFactory>(() =>
            LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
#pragma warning disable IDISP004
                builder.AddProvider(new XunitTestOutputLoggerProvider(output));
#pragma warning restore IDISP004
                builder.SetMinimumLevel(LogLevel.Trace);
            }));

        // Wire up ILogger<T> using the factory
        container.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);
        container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
        container.AddAutoMapper(typeof(Source3));

        var mapper = container.GetInstance<IMapper>();

        var source = new Source3 { Value = 3 };

        var dest = mapper.Map<Dest3>(source);

        dest.Value.Should().Be(source.Value);
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
