# Quick.Mapper 🚀

[![NuGet](https://img.shields.io/nuget/v/Quick.Mapper.svg)](https://www.nuget.org/packages/Quick.Mapper/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Quick.Mapper.svg)](https://www.nuget.org/packages/Quick.Mapper/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Un mapper rápido, eficiente y fácil de usar para .NET, con API idéntica a AutoMapper pero optimizado para rendimiento.

## ✨ Características

- 🎯 **API Idéntica a AutoMapper**: Sintaxis 100% compatible con AutoMapper
- ⚡ **Alto Rendimiento**: Optimizado para mapeos rápidos
- 🔧 **Configuración Flexible**: Mapeos automáticos y personalizados
- 📦 **Dependency Injection**: Integración nativa con Microsoft.Extensions.DependencyInjection
- 🎨 **Perfiles**: Organiza tus mapeos en perfiles reutilizables
- 🔄 **ReverseMap**: Soporte completo para mapeo inverso automático
- 🎭 **Hooks**: BeforeMap y AfterMap para lógica personalizada
- 🛡️ **Type Safety**: Fuertemente tipado con soporte completo para genéricos
- 📝 **Constructor en Perfiles**: Define mapeos directamente en el constructor del Profile

## 📦 Instalación

### Via NuGet Package Manager
```bash
Install-Package Quick.Mapper
```

### Via .NET CLI
```bash
dotnet add package Quick.Mapper
```

### Via PackageReference
```xml
<PackageReference Include="Quick.Mapper" Version="8.0.1" />
```

## 🚀 Inicio Rápido

### Configuración Básica

```csharp
using Quick.Mapper;
using Quick.Mapper.Configuration;

// Crear configuración
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>();
    cfg.CreateMap<Order, OrderDto>();
});

// Crear mapper
var mapper = config.CreateMapper();

// Mapear objetos
var userDto = mapper.Map<UserDto>(user);
```

### Con Dependency Injection

```csharp
// En Program.cs o Startup.cs
using Quick.Mapper.Extensions;

builder.Services.AddQuickMapper(cfg =>
{
    cfg.CreateMap<User, UserDto>();
    cfg.CreateMap<Order, OrderDto>();
});

// Usar en tus servicios
public class UserService
{
    private readonly IMapper _mapper;

    public UserService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public UserDto GetUser(int id)
    {
        var user = _repository.GetById(id);
        return _mapper.Map<UserDto>(user);
    }
}
```

## 📚 Ejemplos de Uso

### Mapeo Simple

```csharp
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

// Configuración
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>();
});

var mapper = config.CreateMapper();
var userDto = mapper.Map<UserDto>(user);
```

### Mapeo Personalizado con ForMember

```csharp
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>()
        .ForMember(dest => dest.FullName, 
            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        .ForMember(dest => dest.Age, 
            opt => opt.MapFrom(src => DateTime.Now.Year - src.BirthYear))
        .ForMember(dest => dest.IsActive, 
            opt => opt.Ignore());
});
```

### Mapeo Condicional

```csharp
cfg.CreateMap<User, UserDto>()
    .ForMember(dest => dest.Email, opt => 
    {
        opt.Condition(src => src.IsEmailPublic);
        opt.MapFrom(src => src.Email);
    });
```

### Mapeo con Valor Constante

```csharp
cfg.CreateMap<User, UserDto>()
    .ForMember(dest => dest.Status, opt => opt.UseValue("Active"))
    .ForMember(dest => dest.CreatedBy, opt => opt.UseValue("System"));
```

### Hooks: BeforeMap y AfterMap

```csharp
cfg.CreateMap<User, UserDto>()
    .BeforeMap((src, dest) => 
    {
        Console.WriteLine($"Mapeando usuario {src.Id}");
    })
    .AfterMap((src, dest) => 
    {
        dest.MappedAt = DateTime.UtcNow;
        dest.Version = "1.0";
    });
```

### Constructor Personalizado

```csharp
cfg.CreateMap<User, UserDto>()
    .ConstructUsing(src => new UserDto(src.Id, src.Email));
```

### Mapeo Inverso (ReverseMap)

```csharp
cfg.CreateMap<User, UserDto>()
    .ForMember(dest => dest.FullName, 
        opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
    .ReverseMap(); // ✅ Crea automáticamente UserDto -> User
```

### Mapeo a Objeto Existente

```csharp
var existingDto = new UserDto();
mapper.Map(user, existingDto); // Actualiza el objeto existente
```

## 🎨 Usando Perfiles

Los perfiles permiten organizar configuraciones de mapeo relacionadas. La sintaxis es **idéntica a AutoMapper**:

```csharp
using Quick.Mapper.Profiles;

public class UserProfile : Profile
{
    public UserProfile()  // ✅ Define mapeos en el constructor
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ReverseMap();
        
        CreateMap<Address, AddressDto>()
            .ReverseMap();
    }
}

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
```

### Registrar Perfiles

```csharp
// Opción 1: Manualmente
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<OrderProfile>();
});

// Opción 2: Con Dependency Injection
builder.Services.AddQuickMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<OrderProfile>();
});
```

### Auto-descubrimiento de Perfiles

Quick.Mapper puede escanear automáticamente todos los perfiles en un ensamblado:

```csharp
// Escanea todos los perfiles en el ensamblado actual
builder.Services.AddQuickMapper(Assembly.GetExecutingAssembly());

// Escanea usando un tipo marcador
builder.Services.AddQuickMapper<Program>();

// Escanea múltiples ensamblados
builder.Services.AddQuickMapper(
    Assembly.GetExecutingAssembly(),
    typeof(SomeOtherType).Assembly
);
```

### Registrar perfiles por tipo

```csharp
builder.Services.AddQuickMapper(
    typeof(UserProfile),
    typeof(OrderProfile),
    typeof(ProductProfile)
);
```

## 🔧 Características Avanzadas

### Validación de Configuración

```csharp
var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>();
});

// Valida que todos los mapeos son correctos
config.AssertConfigurationIsValid();
```

### Obtener Mapeos Registrados

```csharp
var typeMaps = config.GetAllTypeMaps();
foreach (var typeMap in typeMaps)
{
    Console.WriteLine($"{typeMap.SourceType.Name} -> {typeMap.DestinationType.Name}");
}
```

### Mapeo No Genérico

Útil cuando trabajas con reflexión o tipos dinámicos:

```csharp
Type sourceType = typeof(User);
Type destType = typeof(UserDto);

object result = mapper.Map(user, sourceType, destType);
```

### Mapeo a Objeto Existente (No Genérico)

```csharp
mapper.Map(user, existingDto, typeof(User), typeof(UserDto));
```

## 📖 Ejemplos Completos

### Ejemplo: Sistema de Usuario

```csharp
// Modelos
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsEmailPublic { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// Profile
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => DateTime.Now.Year - src.BirthDate.Year))
            .ForMember(dest => dest.Email, opt =>
            {
                opt.Condition(src => src.IsEmailPublic);
                opt.MapFrom(src => src.Email);
            });
    }
}

// Configuración en Startup
builder.Services.AddQuickMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
});

// Uso en Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    public UsersController(IMapper mapper, IUserRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUser(int id)
    {
        var user = _repository.GetById(id);
        if (user == null)
            return NotFound();

        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpPost]
    public ActionResult<UserDto> CreateUser(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        _repository.Add(user);
        
        var userDto = _mapper.Map<UserDto>(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }
}
```

### Ejemplo: Mapeo Bidireccional con ReverseMap

```csharp
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.TotalStock,
                opt => opt.MapFrom(src => src.Stock + src.ReservedStock))
            .ReverseMap()  // Crea automáticamente ProductDto -> Product
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.ReservedStock, opt => opt.Ignore());
    }
}
```

## 🆚 Comparación con AutoMapper

| Característica | Quick.Mapper | AutoMapper |
|---------------|-------------|------------|
| API Sintaxis | ✅ 100% Compatible | ✅ |
| Perfiles con Constructor | ✅ | ✅ |
| ReverseMap | ✅ | ✅ |
| ForMember | ✅ | ✅ |
| BeforeMap/AfterMap | ✅ | ✅ |
| ConstructUsing | ✅ | ✅ |
| Dependency Injection | ✅ | ✅ |
| Validación | ✅ | ✅ |
| Rendimiento | ⚡ Optimizado | ✅ |
| Tamaño del paquete | 📦 Ligero | 📦 Más grande |
| Curva de aprendizaje | ✅ Cero (si conoces AutoMapper) | ✅ |

**Si ya conoces AutoMapper, puedes usar Quick.Mapper sin cambiar NADA de tu código.** 🎉

## 🎓 Guía de Migración desde AutoMapper

Quick.Mapper tiene una **API 100% compatible** con AutoMapper. Para migrar:

1. Cambia el package reference:
```xml
<!-- Antes -->
<PackageReference Include="AutoMapper" Version="X.X.X" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="X.X.X" />

<!-- Después -->
<PackageReference Include="Quick.Mapper" Version="1.0.0" />
```

2. Actualiza los usings:
```csharp
// Antes
using AutoMapper;

// Después
using Quick.Mapper;
using Quick.Mapper.Configuration;
using Quick.Mapper.Profiles;
```

3. **¡Listo!** Tu código existente funcionará sin cambios. ✅

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

## 🤝 Contribuir

Las contribuciones son bienvenidas! Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 🙏 Agradecimientos

- Inspirado en AutoMapper
- Gracias a la comunidad .NET

## 📞 Soporte

- 🐛 Issues: [GitHub Issues](https://github.com/tuusuario/Quick.Mapper/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/tuusuario/Quick.Mapper/discussions)

---

⭐ Si te gusta Quick.Mapper, considera darle una estrella en GitHub!

**Made with ❤️ for the .NET Community**