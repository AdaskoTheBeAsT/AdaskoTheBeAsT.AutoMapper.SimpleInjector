using System.Linq;
using System.Reflection;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
    public class ServiceLifetimeTests
    {
        [Fact]
        public void AddAutoMapperExtensionDefaultWithAssemblySingleDelegateArgCollection()
        {
            // arrange
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(_ => { });
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Should().Be(Lifestyle.Singleton);
            }
        }

        [Fact]
        public void AddAutoMapperExtensionDefaultWithMapperAssemblyMarkerType()
        {
            // arrange
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(typeof(ServiceLifetimeTests));
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Should().Be(Lifestyle.Singleton);
            }
        }

        [Fact]
        public void AddAutoMapperExtensionDefaultWithAssemblyCollection()
        {
            // arrange
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(typeof(ServiceLifetimeTests).GetTypeInfo().Assembly);
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Should().Be(Lifestyle.Singleton);
            }
        }

        [Fact]
        public void AddAutoMapperExtensionTransientWithAssemblySingleDelegateArgCollection()
        {
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(
                cfg => cfg.AsTransient());
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Should().Be(Lifestyle.Transient);
            }
        }

        [Fact]
        public void AddAutoMapperExtensionScopedWithAssemblySingleDelegateArgCollection()
        {
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(
                cfg => cfg.AsScoped());
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Name.Should().Be("Thread Scoped");
            }
        }

        [Fact]
        public void AddAutoMapperExtensionSingletonWithAssemblySingleDelegateArgCollection()
        {
            // arrange
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // act
            container.AddAutoMapper(
                cfg => cfg.AsSingleton());
            var serviceDescriptor = container.GetCurrentRegistrations()
                .FirstOrDefault(r => r.ServiceType == typeof(IMapper));

            // assert
            using (new AssertionScope())
            {
                serviceDescriptor.Should().NotBeNull();

                // ReSharper disable once PossibleNullReferenceException
                serviceDescriptor.Lifestyle.Should().Be(Lifestyle.Singleton);
            }
        }
    }
}
