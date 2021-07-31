using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Integrations
{
#pragma warning disable CA1812
    public sealed class ServiceLifetimeTests
    {
        internal interface ISingletonService
        {
            Bar DoTheThing(Foo theObj);
        }

        [Fact]
        public void ShouldThrowExceptionWhenNullMapperConfigurationPassed()
        {
            // Arrange
            using var container = new Container();
            const Action<Container, IMapperConfigurationExpression>? mapperConfigurationExpressionAction = null;
            Action action = () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                container.AddAutoMapper(
                    cfg =>
                    {
                        cfg.WithMapperAssemblyMarkerTypes(typeof(ServiceLifetimeTests));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        cfg.WithMapperConfigurationExpressionAction(mapperConfigurationExpressionAction);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    });
            };

            // Act & Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanUseDefaultInjectedIMapperInSingletonService()
        {
            // Arrange
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
            container.Register<ISingletonService, TestSingletonService>(Lifestyle.Singleton);
            container.AddAutoMapper(
                cfg =>
                {
                    cfg.WithMapperAssemblyMarkerTypes(typeof(ServiceLifetimeTests));
                    cfg.WithMapperConfigurationExpressionAction(
                        (
                            _,
                            expression) => expression.CreateMap<Foo, Bar>().ReverseMap());
                });
            Bar actual;

            // Act
            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var service = container.GetInstance<ISingletonService>();
                actual = service.DoTheThing(new Foo { TheValue = 1 });
            }

            // Assert
            actual.Should().NotBeNull();
            actual.TheValue.Should().Be(1);
        }

        internal sealed class TestSingletonService : ISingletonService
        {
            private readonly IMapper _mapper;

            public TestSingletonService(IMapper mapper)
            {
                _mapper = mapper;
            }

            public Bar DoTheThing(Foo theObj)
            {
                var bar = _mapper.Map<Bar>(theObj);
                return bar;
            }
        }

        internal sealed class Foo
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public int TheValue { get; set; }
        }

        internal sealed class Bar
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public int TheValue { get; set; }
        }
    }
#pragma warning restore CA1812
}
