using Quick.Mapper.Configuration;
using Quick.Mapper.Internal;

namespace Quick.Mapper.Mapping;

/// <summary>
/// Implementación principal del mapper que realiza las transformaciones entre objetos
/// </summary>
public class Mapper : IMapper
{
    private readonly MapperConfiguration _configuration;
    private readonly MappingEngine _engine;

    /// <summary>
    /// Crea una nueva instancia del mapper
    /// </summary>
    /// <param name="configuration">Configuración del mapper</param>
    public Mapper(MapperConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _engine = new MappingEngine(configuration);
    }

    /// <summary>
    /// Mapea un objeto de origen a un nuevo objeto de destino
    /// </summary>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    public TDestination Map<TDestination>(object source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var sourceType = source.GetType();
        var destinationType = typeof(TDestination);

        return (TDestination)_engine.Map(source, sourceType, destinationType);
    }

    /// <summary>
    /// Mapea un objeto de origen a un nuevo objeto de destino con tipos genéricos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return _engine.Map<TSource, TDestination>(source);
    }

    /// <summary>
    /// Mapea un objeto de origen a un objeto de destino existente
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <param name="destination">Objeto de destino existente</param>
    /// <returns>Objeto de destino con los datos mapeados</returns>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        return _engine.Map(source, destination);
    }

    /// <summary>
    /// Mapea un objeto usando tipos en lugar de genéricos
    /// </summary>
    /// <param name="source">Objeto de origen</param>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    public object Map(object source, Type sourceType, Type destinationType)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (sourceType == null)
            throw new ArgumentNullException(nameof(sourceType));
        if (destinationType == null)
            throw new ArgumentNullException(nameof(destinationType));

        return _engine.Map(source, sourceType, destinationType);
    }

    /// <summary>
    /// Mapea un objeto a un objeto de destino existente usando tipos
    /// </summary>
    /// <param name="source">Objeto de origen</param>
    /// <param name="destination">Objeto de destino existente</param>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Objeto de destino con los datos mapeados</returns>
    public object Map(object source, object destination, Type sourceType, Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(destinationType);

        // Validar tipos
        if (!sourceType.IsInstanceOfType(source))
            throw new ArgumentException($"El objeto source no es del tipo {sourceType.Name}", nameof(source));
        
        if (!destinationType.IsInstanceOfType(destination))
            throw new ArgumentException($"El objeto destination no es del tipo {destinationType.Name}", nameof(destination));

        // Usar reflexión para llamar al método genérico Map<TSource, TDestination>
        var mapMethod = typeof(MappingEngine).GetMethod(nameof(MappingEngine.Map), 
            new[] { sourceType, destinationType });
        
        if (mapMethod == null)
        {
            throw new InvalidOperationException("No se pudo encontrar el método Map");
        }

        var genericMethod = mapMethod.MakeGenericMethod(sourceType, destinationType);
        return genericMethod.Invoke(_engine, new[] { source, destination })!;
    }
}