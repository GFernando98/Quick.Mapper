using System.Linq.Expressions;
using Quick.Mapper.Configuration;
using Quick.Mapper.Exceptions;

namespace Quick.Mapper.Internal;

/// <summary>
/// Motor que ejecuta los mapeos entre objetos
/// </summary>
internal class MappingEngine
{
    private readonly MapperConfiguration _configuration;

    /// <summary>
    /// Crea una nueva instancia del motor de mapeo
    /// </summary>
    public MappingEngine(MapperConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Mapea un objeto de origen a un nuevo objeto de destino
    /// </summary>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var typeMap = FindTypeMap(typeof(TSource), typeof(TDestination));
        
        var destination = CreateDestinationObject<TDestination>(typeMap, source);
        MapProperties(typeMap, source, destination);
        
        return destination;
    }

    /// <summary>
    /// Mapea un objeto de origen a un objeto de destino existente
    /// </summary>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        var typeMap = FindTypeMap(typeof(TSource), typeof(TDestination));
        MapProperties(typeMap, source, destination);
        
        return destination;
    }

    /// <summary>
    /// Mapea usando tipos en lugar de genéricos
    /// </summary>
    public object Map(object source, Type sourceType, Type destinationType)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var typeMap = FindTypeMap(sourceType, destinationType);
        
        var destination = CreateDestinationObject(typeMap, source, destinationType);
        MapProperties(typeMap, source, destination);
        
        return destination;
    }

    /// <summary>
    /// Busca un TypeMap en la configuración
    /// </summary>
    private TypeMap FindTypeMap(Type sourceType, Type destinationType)
    {
        var typeMap = _configuration.FindTypeMapFor(sourceType, destinationType);
        
        if (typeMap == null)
        {
            throw new MappingNotFoundException(sourceType, destinationType);
        }

        return typeMap;
    }

    /// <summary>
    /// Crea una instancia del objeto de destino
    /// </summary>
    private TDestination CreateDestinationObject<TDestination>(TypeMap typeMap, object source)
    {
        // Si hay un constructor personalizado, usarlo
        if (typeMap.CustomConstructor != null)
        {
            var compiled = typeMap.CustomConstructor.Compile();
            return (TDestination)compiled.DynamicInvoke(source)!;
        }

        // Crear instancia usando el constructor por defecto
        return Activator.CreateInstance<TDestination>();
    }

    /// <summary>
    /// Crea una instancia del objeto de destino sin genéricos
    /// </summary>
    private object CreateDestinationObject(TypeMap typeMap, object source, Type destinationType)
    {
        // Si hay un constructor personalizado, usarlo
        if (typeMap.CustomConstructor != null)
        {
            var compiled = typeMap.CustomConstructor.Compile();
            return compiled.DynamicInvoke(source)!;
        }

        // Crear instancia usando el constructor por defecto
        return Activator.CreateInstance(destinationType)!;
    }

    /// <summary>
    /// Mapea todas las propiedades de origen a destino
    /// </summary>
    private void MapProperties(TypeMap typeMap, object source, object destination)
    {
        // Ejecutar BeforeMap si está configurado
        if (typeMap.BeforeMap != null)
        {
            var beforeMapCompiled = typeMap.BeforeMap.Compile();
            beforeMapCompiled.DynamicInvoke(source, destination);
        }

        // Mapear cada propiedad
        foreach (var propertyMap in typeMap.PropertyMaps)
        {
            if (propertyMap.IsIgnored)
                continue;

            try
            {
                MapProperty(propertyMap, source, destination);
            }
            catch (Exception ex)
            {
                throw new QuickMapperException(
                    $"Error al mapear la propiedad '{propertyMap.DestinationProperty.Name}' " +
                    $"de '{typeMap.SourceType.Name}' a '{typeMap.DestinationType.Name}'", ex);
            }
        }

        // Ejecutar AfterMap si está configurado
        if (typeMap.AfterMap != null)
        {
            var afterMapCompiled = typeMap.AfterMap.Compile();
            afterMapCompiled.DynamicInvoke(source, destination);
        }
    }

    /// <summary>
    /// Mapea una propiedad individual
    /// </summary>
    private void MapProperty(PropertyMap propertyMap, object source, object destination)
    {
        object? value;

        // Si hay un mapper personalizado, usarlo
        if (propertyMap.CustomMapper != null)
        {
            var compiled = propertyMap.CustomMapper.Compile();
            value = compiled.DynamicInvoke(source);
        }
        else
        {
            // Obtener el valor de la propiedad de origen
            value = propertyMap.GetSourceValue(source);
        }

        // Evaluar la condición si existe
        if (propertyMap.Condition != null)
        {
            var conditionCompiled = propertyMap.Condition.Compile();
            var shouldMap = (bool)conditionCompiled.DynamicInvoke(source)!;
            
            if (!shouldMap)
                return;
        }

        // Convertir el valor si es necesario
        if (value != null)
        {
            var sourceValueType = value.GetType();
            var targetType = propertyMap.DestinationProperty.PropertyType;

            if (sourceValueType != targetType)
            {
                value = ConvertValue(value, sourceValueType, targetType);
            }
        }

        // Establecer el valor en la propiedad de destino
        propertyMap.SetDestinationValue(destination, value);
    }

    /// <summary>
    /// Convierte un valor de un tipo a otro
    /// </summary>
    private object? ConvertValue(object value, Type sourceType, Type targetType)
    {
        // Manejar Nullable
        var targetUnderlyingType = Nullable.GetUnderlyingType(targetType);
        if (targetUnderlyingType != null)
        {
            if (value == null)
                return null;
            
            targetType = targetUnderlyingType;
        }

        // Si el tipo de destino es asignable desde el tipo de origen
        if (targetType.IsAssignableFrom(sourceType))
            return value;

        // Intentar conversión usando Convert
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            // Si falla, intentar cast directo
            return value;
        }
    }
}