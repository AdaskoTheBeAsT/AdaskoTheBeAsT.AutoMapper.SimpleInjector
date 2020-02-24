using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector;
using AutoMapper;
using SimpleInjector;

namespace TestApp
{
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1402 // File may only contain a single type
    public static class Program
    {
        public static void Main()
        {
            using var container = new Container();
            container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
            container.AddAutoMapper(
                cfg =>
                {
                    cfg.WithMapperAssemblyMarkerTypes(typeof(Program));
                });
            var mapper = container.GetInstance<IMapper>();

            foreach (var typeMap in mapper.ConfigurationProvider.GetAllTypeMaps())
            {
                Console.WriteLine($"{typeMap.SourceType.Name} -> {typeMap.DestinationType.Name}");
            }

            foreach (var service in container.GetCurrentRegistrations())
            {
                Console.WriteLine(service.ServiceType + " - " + service.Registration.ImplementationType);
            }

            var dest = mapper.Map<Dest2>(new Source2());
            Console.WriteLine(dest.ResolvedValue);

            Console.ReadKey();
        }
    }

    public class Source
    {
    }

    public class Dest
    {
    }

    public class Source2
    {
    }

    public class Dest2
    {
        public int ResolvedValue { get; set; }
    }

    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }

    public class Profile2 : Profile
    {
        public Profile2()
        {
            CreateMap<Source2, Dest2>()
                .ForMember(d => d.ResolvedValue, opt => opt.MapFrom<DependencyResolver>());
        }
    }

    public class DependencyResolver : IValueResolver<object, object, int>
    {
        private readonly ISomeService _service;

        public DependencyResolver(ISomeService service)
        {
            _service = service;
        }

        public int Resolve(object source, object destination, int destMember, ResolutionContext context)
        {
            return _service.Modify(destMember);
        }
    }

    public interface ISomeService
    {
        int Modify(int value);
    }

    public class FooService : ISomeService
    {
        private readonly int _value;

        public FooService(int value)
        {
            _value = value;
        }

        public int Modify(int value) => value + _value;
    }
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore SA1201 // Elements should appear in the correct order
}
