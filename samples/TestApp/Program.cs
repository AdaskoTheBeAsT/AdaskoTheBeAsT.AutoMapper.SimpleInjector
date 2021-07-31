using System;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector;
using AutoMapper;
using SimpleInjector;

namespace TestApp
{
    public static class Program
    {
        public static void Main()
        {
#pragma warning disable CC0022 // Should dispose object
            using var container = new Container();
#pragma warning restore CC0022 // Should dispose object
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
                Console.WriteLine($"{service.ServiceType} - {service.Registration.ImplementationType}");
            }

            var dest = mapper.Map<Dest2>(new Source2());
            Console.WriteLine(dest.ResolvedValue);

            Console.ReadKey();
        }
    }
}
