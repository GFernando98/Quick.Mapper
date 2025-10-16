using System.Linq.Expressions;
using System.Reflection;

namespace Quick.Mapper.Internal;

/// <summary>
/// Representa el mapeo completo entre dos tipos
/// </summary>
public class TypeMap
{
    private readonly List<PropertyMap> _propertyMaps = new();
    private bool _isSealed;

    /// <summary>
    /// Par de tipos (origen y destino)
    /// </summary>
    public TypePair TypePair { get; }

    /// <summary>
    /// Tipo de origen
    /// </summary>
    public Type SourceType { get; }

    /// <summary>
    /// Tipo de destino
    /// </summary>
    public Type DestinationType { get; }

    /// <summary>
    /// Mapeos de propiedades individuales
    /// </summary>
    public IReadOnlyList<PropertyMap> PropertyMaps => _propertyMaps.AsReadOnly();

    /// <summary>
    /// Función personalizada para construir el objeto destino
    /// </summary>
    public LambdaExpression? CustomConstructor { get; set; }

    /// <summary>
    /// Acción que se ejecuta antes del mapeo
    /// </summary>
    public LambdaExpression? BeforeMap { get; set; }

    /// <summary>
    /// Acción que se ejecuta después del mapeo
    /// </summary>
    public LambdaExpression? AfterMap { get; set; }

    /// <summary>
    /// Crea un nuevo mapeo de tipos
    /// </summary>
    public TypeMap(TypePair typePair, Type sourceType, Type destinationType)
    {
        TypePair = typePair;
        SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
    }

    /// <summary>
    /// Agrega un mapeo de propiedad
    /// </summary>
    public void AddPropertyMap(PropertyMap propertyMap)
    {
        if (_isSealed)
            throw new InvalidOperationException("No se pueden agregar mapeos después de sellar el TypeMap");

        _propertyMaps.Add(propertyMap);
    }

    /// <summary>
    /// Busca un mapeo de propiedad por el nombre de la propiedad de destino
    /// </summary>
    public PropertyMap? FindPropertyMapFor(string destinationPropertyName)
    {
        return _propertyMaps.FirstOrDefault(pm => 
            pm.DestinationProperty.Name.Equals(destinationPropertyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Busca un mapeo de propiedad por PropertyInfo
    /// </summary>
    public PropertyMap? FindPropertyMapFor(PropertyInfo destinationProperty)
    {
        return _propertyMaps.FirstOrDefault(pm => pm.DestinationProperty == destinationProperty);
    }

    /// <summary>
    /// Obtiene o crea un mapeo de propiedad
    /// </summary>
    public PropertyMap GetOrCreatePropertyMap(PropertyInfo destinationProperty)
    {
        var propertyMap = FindPropertyMapFor(destinationProperty);
        if (propertyMap == null)
        {
            propertyMap = new PropertyMap(destinationProperty);
            AddPropertyMap(propertyMap);
        }
        return propertyMap;
    }

    /// <summary>
    /// Sella el TypeMap y genera los mapeos automáticos
    /// </summary>
    public void Seal()
    {
        if (_isSealed)
            return;

        BuildAutomaticPropertyMaps();
        _isSealed = true;
    }

    /// <summary>
    /// Construye mapeos automáticos para propiedades que no están configuradas
    /// </summary>
    private void BuildAutomaticPropertyMaps()
    {
        var destinationProperties = DestinationType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        var sourceProperties = SourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var destProperty in destinationProperties)
        {
            var propertyMap = FindPropertyMapFor(destProperty);
            
            // Si ya existe configuración para esta propiedad, continuar
            if (propertyMap != null)
                continue;

            // Intentar encontrar una propiedad de origen con el mismo nombre
            if (sourceProperties.TryGetValue(destProperty.Name, out var sourceProperty))
            {
                // Verificar compatibilidad de tipos
                if (AreTypesCompatible(sourceProperty.PropertyType, destProperty.PropertyType))
                {
                    propertyMap = new PropertyMap(destProperty)
                    {
                        SourceProperty = sourceProperty
                    };
                    AddPropertyMap(propertyMap);
                }
            }
        }
    }

    /// <summary>
    /// Verifica si dos tipos son compatibles para mapeo
    /// </summary>
    private bool AreTypesCompatible(Type sourceType, Type destinationType)
    {
        // Mismo tipo
        if (sourceType == destinationType)
            return true;

        // Tipos asignables
        if (destinationType.IsAssignableFrom(sourceType))
            return true;

        // Nullable a no-nullable del mismo tipo subyacente
        var sourceUnderlyingType = Nullable.GetUnderlyingType(sourceType);
        var destUnderlyingType = Nullable.GetUnderlyingType(destinationType);

        if (sourceUnderlyingType != null && sourceUnderlyingType == destinationType)
            return true;

        if (destUnderlyingType != null && destUnderlyingType == sourceType)
            return true;

        if (sourceUnderlyingType != null && destUnderlyingType != null && 
            sourceUnderlyingType == destUnderlyingType)
            return true;

        // Conversiones implícitas de tipos numéricos
        if (IsNumericType(sourceType) && IsNumericType(destinationType))
            return true;

        return false;
    }

    /// <summary>
    /// Verifica si un tipo es numérico
    /// </summary>
    private bool IsNumericType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        return underlyingType == typeof(byte) ||
               underlyingType == typeof(sbyte) ||
               underlyingType == typeof(short) ||
               underlyingType == typeof(ushort) ||
               underlyingType == typeof(int) ||
               underlyingType == typeof(uint) ||
               underlyingType == typeof(long) ||
               underlyingType == typeof(ulong) ||
               underlyingType == typeof(float) ||
               underlyingType == typeof(double) ||
               underlyingType == typeof(decimal);
    }

    /// <summary>
    /// Valida que el TypeMap es correcto
    /// </summary>
    public void Validate()
    {
        if (!_isSealed)
            throw new InvalidOperationException("El TypeMap debe estar sellado antes de validar");

        foreach (var propertyMap in _propertyMaps)
        {
            propertyMap.Validate();
        }
    }
}