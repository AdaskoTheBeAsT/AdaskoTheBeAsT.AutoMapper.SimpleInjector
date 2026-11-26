using System;
using System.Globalization;
using AdaskoTheBeAsT.AutoMapper.SimpleInjector;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            container.RegisterInstance<ILoggerFactory>(NullLoggerFactory.Instance);
            container.Register<ISomeService>(() => new FooService(5), Lifestyle.Transient);
            container.AddAutoMapper(typeof(Profile1));

            container.Verify();
            container.GetInstance<IConfigurationProvider>().AssertConfigurationIsValid();
            var mapper = container.GetInstance<IMapper>();

            var dest = mapper.Map<Dest>(new Source { Value = 7 });
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Mapped value: {dest.Value}"));
            var resolved = mapper.Map<Dest2>(new Source2 { Value = 10 });
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"Resolved value: {resolved.ResolvedValue}"));
        }
    }
}
