using System;
using System.Reflection;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
    public class AssemblyResolutionTests
    {
        private static readonly Container _container = BuildContainer();

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
        public void CanRegisterTwiceWithoutProblems()
        {
            new Action(() => BuildContainer()).Should().NotThrow();
        }

        private static Container BuildContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.AddAutoMapper(typeof(Source).GetTypeInfo().Assembly);
            return container;
        }
    }
}
