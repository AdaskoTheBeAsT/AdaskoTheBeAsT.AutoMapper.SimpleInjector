using System;
using System.Reflection;
using AutoMapper;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Should().Be(Lifestyle.Singleton);
#pragma warning restore S8969
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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Should().Be(Lifestyle.Singleton);
#pragma warning restore S8969
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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Should().Be(Lifestyle.Singleton);
#pragma warning restore S8969
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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Should().Be(Lifestyle.Transient);
#pragma warning restore S8969
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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Name.Should().Be("Thread Scoped");
#pragma warning restore S8969
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
        var serviceDescriptor = Array.Find(
            container.GetCurrentRegistrations(),
            r => r.ServiceType == typeof(IMapper));

        // assert
        using (new AssertionScope())
        {
            serviceDescriptor.Should().NotBeNull();

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable S8969
            serviceDescriptor!.Lifestyle.Should().Be(Lifestyle.Singleton);
#pragma warning restore S8969
        }
    }
}
