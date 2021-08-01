# AdaskoTheBeAsT.AutoMapper.SimpleInjector

AutoMapper extensions for SimpleInjector

## Badges

[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.automapper.simpleinjector/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.automapper.simpleinjector)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector/alerts/)
[![Build Status](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.AutoMapper.SimpleInjector/_apis/build/status/AdaskoTheBeAsT.AdaskoTheBeAsT.AutoMapper.SimpleInjector?branchName=master)](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.AutoMapper.SimpleInjector/_build/latest?definitionId=8&branchName=master)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector)
![Sonar Tests](https://img.shields.io/sonar/tests/AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Count](https://img.shields.io/sonar/total_tests/AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Execution Time](https://img.shields.io/sonar/test_execution_time/AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector?server=https%3A%2F%2Fsonarcloud.io&style=plastic)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.AutoMapper.SimpleInjector)

## Usage

Scans assemblies and:

1. adds profiles to mapping configuration
2. adds implementations of `ITypeConverter`, `IValueConverter`, `IValueResolver`, `IMemberValueResolver`,`IMappingAction` instances as transient to the container.

There are few options to use with `Container` instance:

1. Marker type from assembly which will be scanned

   ```cs
    container.AddAutoMapper(typeof(MyMapper), type2 /*, ...*/);
   ```

1. List of assemblies which will be scanned.

   Below is sample for scanning assemblies from some solution.

    ```cs
    [ExcludeFromCodeCoverage]
    public static class AutoMapperConfigurator
    {
        private const string NamespacePrefix = "YourNamespace";

        public static void Configure(Container container)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = new List<Assembly>();
            var mainAssembly = typeof(AutoMapperConfigurator).Assembly;
            var refAssemblies = mainAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in refAssemblies
                .Where(a => a.FullName.StartsWith(NamespacePrefix, StringComparison.OrdinalIgnoreCase)))
                {
                    var assembly = loadedAssemblies.Find(l => l.FullName == assemblyName.FullName)
                        ?? AppDomain.CurrentDomain.Load(assemblyName);
                    assemblies.Add(assembly);
                }
            container.AddAutoMapper(assemblies);
        }
    }
   ```

This registers AutoMapper:

- `MapperConfiguration` - As a singleton
- `IMapper` - As a singleton
- `ITypeConverter` instances as transient
- `IValueConverter` instances as transient
- `IValueResolver` instances as transient
- `IMemberValueResolver` instances as transient
- `IMappingAction` instances as transient

Mapping configuration is static as it is the root object that can create an `IMapper`.

Mapper instance are registered as singleton. You can configure this with the `Lifestyle` parameter. Be careful changing this, as `Mapper` takes a dependency on a factory method to instantiate the other extensions.

## Advanced usage

### Setting up custom `IMapper` instance and marker type from assembly for unit testing (Moq sample)

   ```cs
    var testMapper = new Mock<IMapper>();
    container.AddAutoMapper(
        cfg =>
        {
            cfg.Using(() => testMapper.Object);
            cfg.WithMapperAssemblyMarkerTypes(typeof(MyMarkerType));
        });
   ```

### Setting up custom `IMapper` implementation and marker type from assembly

   ```cs
    container.AddAutoMapper(
        cfg =>
        {
            cfg.Using<MyCustomMapper>();
            cfg.WithMapperAssemblyMarkerTypes(typeof(MyMarkerType));
        });
   ```

### Setting up custom `IMapper` implementation and assemblies to scan

   ```cs
    container.AddAutoMapper(
        cfg =>
        {
            cfg.Using<MyCustomMapper>();
            cfg.WithAssembliesToScan(assemblies);
        });
   ```

### Setting assemblies to scan and different lifetime for `IMapper` implementation

   ```cs
    container.AddAutoMapper(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.AsScoped();
        });
   ```

### Setting configuration for MapperConfigurationExpression

   ```cs
    container.AddAutoMapper(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.AsScoped();
            cfg.WithMapperConfigurationExpressionAction(
                        (
                            container1,
                            expression) => expression.CreateMap<Foo, Bar>().ReverseMap());
        });
   ```

Library scans all descendant classes from `Profile` so it is better to store mapping in `Profile` descendants

   ```cs
    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }
   ```

### Mapper.Map usage

To map at runtime, add a dependency on `IMapper`:

```c#
public class EmployeesController {
	private readonly IMapper _mapper;

	public EmployeesController(IMapper mapper)
		=> _mapper = mapper;

	// use _mapper.Map to map
}
```

### ProjectTo usage

Starting with 8.0 you can use `IMapper.ProjectTo`. The old `ProjectTo` is an extension method and does not have dependency injection available. Pass an `IConfigurationProvider` instance directly:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_configurationProvider)
					   .ToListAsync();
```

Or you can use an `IMapper` instance:

```c#
var orders = await dbContext.Orders
                       .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
					   .ToListAsync();
```

# Thanks to:

- Jimmy Boggard for AutoMapper
- Steven van Deursen for SimpleInjector

Code originates from AutoMapper.Extensions.Microsoft.DependencyInjection and was changed to work with SimpleInjector.