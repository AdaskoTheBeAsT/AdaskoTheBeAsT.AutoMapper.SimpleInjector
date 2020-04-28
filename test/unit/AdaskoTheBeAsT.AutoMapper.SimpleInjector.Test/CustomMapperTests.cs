using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using SimpleInjector;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CA1812
    public sealed class CustomMapperTests
        : IDisposable
    {
        private readonly Container _container;

        public CustomMapperTests()
        {
            _container = new Container();
        }

        [Fact]
        public void ShouldResolveMapperWhenCustomMapperTypeProvided()
        {
            // Arrange & Act
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
            Func<IMapper>? instanceCreator = null;

            Action action = () => _container.AddAutoMapper(
#pragma warning disable 8604
                cfg => cfg.Using(instanceCreator));
#pragma warning restore 8604

            // Act & Assert
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

        internal class MyCustomMapper : IMapper
        {
            public MyCustomMapper()
            {
                ConfigurationProvider = null;
                ServiceCtor = null;
            }

            public IConfigurationProvider ConfigurationProvider { get; }

            public Func<Type, object> ServiceCtor { get; }

            public TDestination Map<TDestination>(object source)
            {
                throw new NotImplementedException();
            }

            public TDestination Map<TDestination>(
                object source,
                Action<IMappingOperationOptions> opts)
            {
                throw new NotImplementedException();
            }

            public TDestination Map<TSource, TDestination>(TSource source)
            {
                throw new NotImplementedException();
            }

            public TDestination Map<TSource, TDestination>(
                TSource source,
                Action<IMappingOperationOptions<TSource, TDestination>> opts)
            {
                throw new NotImplementedException();
            }

            public TDestination Map<TSource, TDestination>(
                TSource source,
                TDestination destination)
            {
                throw new NotImplementedException();
            }

            public TDestination Map<TSource, TDestination>(
                TSource source,
                TDestination destination,
                Action<IMappingOperationOptions<TSource, TDestination>> opts)
            {
                throw new NotImplementedException();
            }

            public object Map(
                object source,
                Type sourceType,
                Type destinationType)
            {
                throw new NotImplementedException();
            }

            public object Map(
                object source,
                Type sourceType,
                Type destinationType,
                Action<IMappingOperationOptions> opts)
            {
                throw new NotImplementedException();
            }

            public object Map(
                object source,
                object destination,
                Type sourceType,
                Type destinationType)
            {
                throw new NotImplementedException();
            }

            public object Map(
                object source,
                object destination,
                Type sourceType,
                Type destinationType,
                Action<IMappingOperationOptions> opts)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TDestination> ProjectTo<TDestination>(
                IQueryable source,
                object? parameters = null,
                params Expression<Func<TDestination, object>>[] membersToExpand)
            {
                throw new NotImplementedException();
            }

            public IQueryable<TDestination> ProjectTo<TDestination>(
                IQueryable source,
                IDictionary<string, object> parameters,
                params string[] membersToExpand)
            {
                throw new NotImplementedException();
            }
        }
    }
#pragma warning restore CA1812
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
}
