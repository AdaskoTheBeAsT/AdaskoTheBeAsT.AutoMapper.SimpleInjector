# AdaskoTheBeAsT.AutoMapper.SimpleInjector
AutoMapper extensions for SimpleInjector

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
    public static class MediatRConfigurator
    {
        private const string NamespacePrefix = "YourNamespace";

        public static void Configure(Container container)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var assemblies = new List<Assembly>();
            var mainAssembly = typeof(MediatrConfigurer).Assembly;
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