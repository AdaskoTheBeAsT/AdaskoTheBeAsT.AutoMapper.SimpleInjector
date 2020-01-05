using System;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
    public sealed class AppDomainResolutionTests
        : IDisposable
    {
        private readonly Container _container;

        public AppDomainResolutionTests()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
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
            _container.GetInstance<IConfigurationProvider>().GetAllTypeMaps().Should().HaveCount(3);
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
}
