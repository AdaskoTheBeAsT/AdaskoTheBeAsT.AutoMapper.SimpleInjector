using AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public class ScopeTests
    {
        [Fact]
        public void Can_depend_on_scoped_services_as_transient()
        {
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.AddAutoMapper(
                cfg =>
                {
                    cfg.WithMapperAssemblyMarkerTypes(typeof(Source));
                    cfg.AsTransient();
                });
            container.Register<ISomeService, MutableService>(Lifestyle.Scoped);

            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var mutableService = (MutableService)container.GetInstance<ISomeService>();
                mutableService.Value = 10;

                var mapper = container.GetInstance<IMapper>();

                var dest = mapper.Map<Dest2>(new Source2 { ConvertedValue = 5 });

                dest.ConvertedValue.Should().Be(15);
            }
        }

        [Fact]
        public void Can_depend_on_scoped_services_as_scoped()
        {
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.AddAutoMapper(
                cfg =>
                {
                    cfg.WithMapperAssemblyMarkerTypes(typeof(Source));
                    cfg.AsScoped();
                });
            container.Register<ISomeService, MutableService>(Lifestyle.Scoped);

            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var mutableService = (MutableService)container.GetInstance<ISomeService>();
                mutableService.Value = 10;

                var mapper = container.GetInstance<IMapper>();

                var dest = mapper.Map<Dest2>(new Source2 { ConvertedValue = 5 });

                dest.ConvertedValue.Should().Be(15);
            }
        }

        [Fact]
        public void Cannot_correctly_resolve_scoped_services_as_singleton()
        {
            using var container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            container.AddAutoMapper(
                cfg =>
                {
                    cfg.WithMapperAssemblyMarkerTypes(typeof(Source));
                    cfg.AsSingleton();
                });

            container.Register<ISomeService, MutableService>(Lifestyle.Scoped);

            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var mutableService = (MutableService)container.GetInstance<ISomeService>();
                mutableService.Value = 10;

                var mapper = container.GetInstance<IMapper>();

                var dest = mapper.Map<Dest2>(new Source2 { ConvertedValue = 5 });

                dest.ConvertedValue.Should().Be(15);
            }
        }
    }
#pragma warning restore CA1707 // Identifiers should not contain underscores
}
