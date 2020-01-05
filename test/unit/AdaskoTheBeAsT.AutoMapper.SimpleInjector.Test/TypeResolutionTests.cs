using System;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
    public sealed class TypeResolutionTests
        : IDisposable
    {
        private readonly Container _container;

        public TypeResolutionTests()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
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
            _container.GetInstance<IConfigurationProvider>().GetAllTypeMaps().Should().HaveCount(3);
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

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
