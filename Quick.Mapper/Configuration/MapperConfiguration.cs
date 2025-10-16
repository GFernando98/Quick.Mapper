using Quick.Mapper.Exceptions;
using Quick.Mapper.Internal;
using Quick.Mapper.Mapping;

namespace Quick.Mapper.Configuration;

/// <summary>
/// Configuración principal del mapper que almacena todos los mapeos registrados
/// </summary>
public class MapperConfiguration : IMapperConfiguration
{
    private readonly List<TypeMap> _typeMaps;
    private readonly object _lockObject = new();
    private bool _isSealed;

    /// <summary>
    /// Crea una nueva configuración de mapper
    /// </summary>
    /// <param name="configAction">Acción para configurar los mapeos</param>
    public MapperConfiguration(Action<MapperConfigurationExpression> configAction)
    {
        if (configAction == null)
            throw new ArgumentNullException(nameof(configAction));

        var expression = new MapperConfigurationExpression();
        configAction(expression);

        _typeMaps = expression.TypeMaps;
        
        Seal();
    }

    /// <summary>
    /// Crea una nueva configuración de mapper desde una expresión existente
    /// </summary>
    internal MapperConfiguration(MapperConfigurationExpression expression)
    {
        _typeMaps = expression.TypeMaps;
        Seal();
    }

    /// <summary>
    /// Sella la configuración para que no pueda ser modificada
    /// </summary>
    private void Seal()
    {
        lock (_lockObject)
        {
            if (_isSealed)
                return;

            // Construir mapeos automáticos si no están configurados
            foreach (var typeMap in _typeMaps)
            {
                typeMap.Seal();
            }

            _isSealed = true;
        }
    }

    /// <summary>
    /// Crea una instancia del mapper basada en esta configuración
    /// </summary>
    public IMapper CreateMapper()
    {
        if (!_isSealed)
            throw new ConfigurationException("La configuración debe estar sellada antes de crear el mapper");

        return new Mapping.Mapper(this);
    }

    /// <summary>
    /// Valida que todas las configuraciones de mapeo son correctas
    /// </summary>
    public void AssertConfigurationIsValid()
    {
        var errors = new List<string>();

        foreach (var typeMap in _typeMaps)
        {
            try
            {
                typeMap.Validate();
            }
            catch (Exception ex)
            {
                errors.Add($"Error en mapeo {typeMap.SourceType.Name} -> {typeMap.DestinationType.Name}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            throw new ConfigurationException(
                $"La configuración tiene {errors.Count} error(es):\n{string.Join("\n", errors)}");
        }
    }

    /// <summary>
    /// Obtiene todos los mapeos de tipos registrados
    /// </summary>
    public IEnumerable<TypeMap> GetAllTypeMaps()
    {
        return _typeMaps.AsReadOnly();
    }

    /// <summary>
    /// Busca un mapeo específico entre dos tipos
    /// </summary>
    internal TypeMap? FindTypeMapFor(Type sourceType, Type destinationType)
    {
        var typePair = new TypePair(sourceType, destinationType);
        return _typeMaps.FirstOrDefault(tm => tm.TypePair.Equals(typePair));
    }

    /// <summary>
    /// Busca un mapeo específico por TypePair
    /// </summary>
    internal TypeMap? FindTypeMapFor(TypePair typePair)
    {
        return _typeMaps.FirstOrDefault(tm => tm.TypePair.Equals(typePair));
    }
}