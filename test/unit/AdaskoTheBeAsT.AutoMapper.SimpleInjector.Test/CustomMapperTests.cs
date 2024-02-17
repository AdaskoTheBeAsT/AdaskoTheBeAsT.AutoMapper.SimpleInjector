using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SimpleInjector;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1812
public sealed class CustomMapperTests
    : IDisposable
{
    private readonly Container _container;

    public CustomMapperTests()
    {
        _container = new Container();
        _container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
    }

    [Fact]
    public void ShouldResolveMapperWhenCustomMapperTypeProvided()
    {
        // Arrange and Act
        _container.AddAutoMapper(
            cfg =>
            {
                cfg.Using<MyCustomMapper>();
                cfg.WithMapperAssemblyMarkerTypes(typeof(CustomMapperTests));
            });

        // Assert
        using (new AssertionScope())
        {
            _container.GetInstance<IMapper>().Should().NotBeNull();
            _container.GetInstance<IMapper>().GetType().Should().Be(typeof(MyCustomMapper));
        }
    }

    [Fact]
    public void ShouldThrowExceptionWhenNullMapperInstanceProvided()
    {
        // Arrange
        const Func<IMapper>? instanceCreator = null;

        Action action = () => _container.AddAutoMapper(
            cfg => cfg.Using(instanceCreator));

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldResolveMapperWhenCustomMapperInstanceProvided()
    {
        // Arrange
        var customMapper = new Mock<IMapper>();

        // Act
        _container.AddAutoMapper(
            cfg =>
            {
                cfg.Using(() => customMapper.Object);
                cfg.WithMapperAssemblyMarkerTypes(typeof(CustomMapperTests));
            });

        // Assert
        using (new AssertionScope())
        {
            _container.GetInstance<IMapper>().Should().NotBeNull();
            _container.GetInstance<IMapper>().GetType().Should().Be(customMapper.Object.GetType());
        }
    }

    public void Dispose()
    {
        _container.Dispose();
    }

    internal sealed class MyCustomMapper
        : IMapper
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MyCustomMapper()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ConfigurationProvider = null;
            ServiceCtor = null;
        }

        public IConfigurationProvider ConfigurationProvider { get; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<Type, object>? ServiceCtor { get; }

        public TDestination Map<TDestination>(object? source) => throw new NotSupportedException();

#pragma warning disable MA0038 // Make method static
        public TDestination Map<TDestination>(
            object source,
            Action<IMappingOperationOptions> opts) =>
            throw new NotSupportedException();
#pragma warning restore MA0038 // Make method static

        public TDestination Map<TSource, TDestination>(TSource? source) => throw new NotSupportedException();

        public TDestination Map<TDestination>(
            object? source,
            Action<IMappingOperationOptions<object, TDestination>> opts) =>
            throw new NotSupportedException();

        public TDestination Map<TSource, TDestination>(
            TSource? source,
            Action<IMappingOperationOptions<TSource, TDestination>> opts) =>
            throw new NotSupportedException();

        public TDestination Map<TSource, TDestination>(
            TSource? source,
            TDestination? destination) =>
            throw new NotSupportedException();

        public TDestination Map<TSource, TDestination>(
            TSource? source,
            TDestination? destination,
            Action<IMappingOperationOptions<TSource, TDestination>> opts) =>
            throw new NotSupportedException();

        public object Map(
            object? source,
            Type? sourceType,
            Type destinationType,
            Action<IMappingOperationOptions<object, object>> opts) =>
            throw new NotSupportedException();

        public object Map(
            object? source,
            object? destination,
            Type? sourceType,
            Type? destinationType,
            Action<IMappingOperationOptions<object, object>> opts) =>
            throw new NotSupportedException();

        public object Map(
            object? source,
            Type? sourceType,
            Type destinationType) =>
            throw new NotSupportedException();

#pragma warning disable MA0038 // Make method static
        public object Map(
            object source,
            Type sourceType,
            Type destinationType,
            Action<IMappingOperationOptions> opts) =>
            throw new NotSupportedException();
#pragma warning restore MA0038 // Make method static

        public object Map(
            object? source,
            object? destination,
            Type? sourceType,
            Type? destinationType) =>
            throw new NotSupportedException();

#pragma warning disable MA0038 // Make method static
        public object Map(
            object source,
            object destination,
            Type sourceType,
            Type destinationType,
            Action<IMappingOperationOptions> opts) =>
            throw new NotSupportedException();
#pragma warning restore MA0038 // Make method static

        public IQueryable<TDestination> ProjectTo<TDestination>(
            IQueryable source,
            object? parameters = null,
            params Expression<Func<TDestination, object>>[] membersToExpand) =>
            throw new NotSupportedException();

        public IQueryable<TDestination> ProjectTo<TDestination>(
            IQueryable source,
            IDictionary<string, object>? parameters,
            params string[] membersToExpand) =>
            throw new NotSupportedException();

        public IQueryable ProjectTo(
            IQueryable source,
            Type destinationType,
            IDictionary<string, object>? parameters = null,
            params string[] membersToExpand) =>
            throw new NotSupportedException();
    }
}
#pragma warning restore CA1812
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
