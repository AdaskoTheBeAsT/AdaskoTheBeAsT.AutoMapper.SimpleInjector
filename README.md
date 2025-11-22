# AdaskoTheBeAsT.AutoMapper.SimpleInjector

> Seamless AutoMapper integration for SimpleInjector with automatic registration and configuration

[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.automapper.simpleinjector/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.automapper.simpleinjector)
[![Build Status](https://img.shields.io/azure-devops/build/adaskothebeast/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16)](https://img.shields.io/azure-devops/build/adaskothebeast/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector/16?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector)
![Sonar Coverage](https://img.shields.io/sonar/coverage/AdaskoTheBeAsT_AdaskoTheBeAsT.AutoMapper.SimpleInjector?server=https%3A%2F%2Fsonarcloud.io&style=plastic)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.AutoMapper.SimpleInjector)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.AutoMapper.SimpleInjector.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.AutoMapper.SimpleInjector?ref=badge_shield)

## Why Use This?

This library eliminates the boilerplate of manually registering AutoMapper with SimpleInjector. It automatically:

- 🔍 **Scans assemblies** for `Profile` classes and registers them
- 🔄 **Auto-registers** all AutoMapper extensions (`ITypeConverter`, `IValueResolver`, `IMemberValueResolver`, `IValueConverter`, `IMappingAction`)
- 💉 **Handles dependency injection** for your custom converters and resolvers
- ⚙️ **Configures everything** with a single fluent API call
- 🎯 **Supports .NET 8, 9, and 10**

## Installation

```bash
dotnet add package AdaskoTheBeAsT.AutoMapper.SimpleInjector
```

### 1. Create Your Mapping Profiles

```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserDto, UserEntity>();
    }
}
```

### 2. Register AutoMapper with SimpleInjector

```csharp
using AdaskoTheBeAsT.AutoMapper.SimpleInjector;
using SimpleInjector;

var container = new Container();

// Option 1: Scan by marker type (recommended)
container.AddAutoMapper(typeof(UserProfile));

// Option 2: Scan multiple assemblies
container.AddAutoMapper(typeof(UserProfile).Assembly, typeof(OrderProfile).Assembly);

// Option 3: Scan with configuration
container.AddAutoMapper(cfg =>
{
    cfg.WithMapperAssemblyMarkerTypes(typeof(UserProfile));
    cfg.AsScoped(); // Change lifestyle if needed
});
```

### 3. Use IMapper in Your Services

```csharp
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public UserDto GetUser(UserEntity entity)
    {
        return _mapper.Map<UserDto>(entity);
    }
}
```

## What Gets Registered?

This library automatically registers the following with your container:

| Type | Registration | Description |
|------|--------------|-------------|
| `IConfigurationProvider` | **Singleton** | AutoMapper configuration |
| `IMapper` | **Singleton*** | Main mapper instance |
| `ITypeConverter<,>` | **Transient** | Custom type converters |
| `IValueConverter<,>` | **Transient** | Custom value converters |
| `IValueResolver<,,>` | **Transient** | Custom value resolvers |
| `IMemberValueResolver<,,,>` | **Transient** | Custom member resolvers |
| `IMappingAction<,>` | **Transient** | Custom mapping actions |

_*Lifestyle is configurable (Singleton/Scoped/Transient)_

## Usage Examples

### Basic Usage - Scan by Type

```csharp
container.AddAutoMapper(typeof(UserProfile), typeof(OrderProfile));
```

### Scan Assemblies from Your Solution

```csharp
public static class AutoMapperConfigurator
{
    private const string NamespacePrefix = "YourCompany.YourProject";

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

### Using Custom Resolvers with Dependency Injection

Your custom resolvers can have dependencies automatically injected:

```csharp
public class DependencyResolver : IValueResolver<Source, Dest, int>
{
    private readonly ISomeService _service;

    public DependencyResolver(ISomeService service)
    {
        _service = service;
    }

    public int Resolve(Source source, Dest dest, int destMember, ResolutionContext context)
    {
        return _service.Modify(destMember);
    }
}

// Registration
container.Register<ISomeService, SomeService>();
container.AddAutoMapper(typeof(DependencyResolver));

// The resolver will automatically get ISomeService injected!
```

## Advanced Configuration

### Change IMapper Lifestyle (Scoped/Transient)

```csharp
container.AddAutoMapper(cfg =>
{
    cfg.WithMapperAssemblyMarkerTypes(typeof(UserProfile));
    cfg.AsScoped();      // Use Scoped lifestyle
    // or cfg.AsTransient(); // Use Transient lifestyle
    // or cfg.AsSingleton(); // Default: Singleton
});
```

### Custom Mapper Configuration

Add custom mappings programmatically:

```csharp
container.AddAutoMapper(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.WithMapperConfigurationExpressionAction((container, expression) =>
    {
        // Add custom mappings
        expression.CreateMap<Foo, Bar>().ReverseMap();
        
        // Configure global settings
        expression.AllowNullCollections = true;
    });
});
```

### Using Custom IMapper Implementation

```csharp
public class MyCustomMapper : IMapper
{
    // Your custom implementation
}

container.AddAutoMapper(cfg =>
{
    cfg.Using<MyCustomMapper>();
    cfg.WithMapperAssemblyMarkerTypes(typeof(UserProfile));
});
```

### Unit Testing with Mock IMapper

Perfect for unit tests using Moq or similar frameworks:

```csharp
var testMapper = new Mock<IMapper>();
testMapper.Setup(m => m.Map<UserDto>(It.IsAny<UserEntity>()))
    .Returns(new UserDto { Id = 1, Name = "Test User" });

container.AddAutoMapper(cfg =>
{
    cfg.Using(() => testMapper.Object);
    cfg.WithMapperAssemblyMarkerTypes(typeof(UserProfile));
});
```

### Setting AutoMapper License Key

If you're using AutoMapper's premium features:

```csharp
container.AddAutoMapper(cfg =>
{
    cfg.WithLicenseKey("your-license-key");
    cfg.WithMapperAssemblyMarkerTypes(typeof(UserProfile));
});
```

## Common Patterns

### Using IMapper.Map

Inject `IMapper` into your services to perform runtime mapping:

```csharp
public class EmployeesController
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _repository;

    public EmployeesController(IMapper mapper, IEmployeeRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public EmployeeDto GetEmployee(int id)
    {
        var employee = _repository.GetById(id);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public IEnumerable<EmployeeDto> GetAllEmployees()
    {
        var employees = _repository.GetAll();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }
}
```

### Using ProjectTo for Entity Framework / LINQ

For efficient database queries, use `ProjectTo` to project directly to DTOs:

```csharp
public class OrderService
{
    private readonly DbContext _dbContext;
    private readonly IConfigurationProvider _configurationProvider;

    public OrderService(DbContext dbContext, IConfigurationProvider configurationProvider)
    {
        _dbContext = dbContext;
        _configurationProvider = configurationProvider;
    }

    // Option 1: Using IConfigurationProvider directly (recommended)
    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await _dbContext.Orders
            .ProjectTo<OrderDto>(_configurationProvider)
            .ToListAsync();
    }
}
```

Or inject `IMapper` instead:

```csharp
public class OrderService
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public OrderService(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    // Option 2: Using IMapper.ConfigurationProvider
    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await _dbContext.Orders
            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
```

## API Reference

### AddAutoMapper Extension Methods

| Method | Description |
|--------|-------------|
| `AddAutoMapper(params Type[])` | Register AutoMapper by scanning assemblies containing the specified types |
| `AddAutoMapper(params Assembly[])` | Register AutoMapper by scanning the specified assemblies |
| `AddAutoMapper(IEnumerable<Assembly>)` | Register AutoMapper by scanning the specified assemblies |
| `AddAutoMapper(IEnumerable<Type>)` | Register AutoMapper by scanning assemblies containing the specified types |
| `AddAutoMapper(Action<AutoMapperSimpleInjectorConfiguration>)` | Register AutoMapper with custom configuration |

### Configuration Methods

| Method | Description |
|--------|-------------|
| `WithMapperAssemblyMarkerTypes(params Type[])` | Specify types to mark assemblies for scanning |
| `WithAssembliesToScan(IEnumerable<Assembly>)` | Specify assemblies to scan directly |
| `Using<TMapper>()` | Use a custom `IMapper` implementation |
| `Using(Func<IMapper>)` | Use a factory function to create `IMapper` (useful for testing) |
| `AsSingleton()` | Register `IMapper` as Singleton (default) |
| `AsScoped()` | Register `IMapper` as Scoped |
| `AsTransient()` | Register `IMapper` as Transient |
| `WithMapperConfigurationExpressionAction(...)` | Add custom mapper configuration |
| `WithLicenseKey(string)` | Set AutoMapper license key |

## Requirements

- **.NET 8.0, 9.0, or 10.0**
- **AutoMapper 15.1.0+**
- **SimpleInjector 5.5.0+**

## Credits

Special thanks to:

- **[Jimmy Bogard](https://github.com/jbogard)** for creating AutoMapper
- **[Steven van Deursen](https://github.com/dotnetjunkie)** for creating SimpleInjector

This library is based on [AutoMapper.Extensions.Microsoft.DependencyInjection](https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection) and adapted to work seamlessly with SimpleInjector.

## License

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.AutoMapper.SimpleInjector.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.AutoMapper.SimpleInjector?ref=badge_large)

---

**Found this helpful? Give it a ⭐ on [GitHub](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.AutoMapper.SimpleInjector)!**