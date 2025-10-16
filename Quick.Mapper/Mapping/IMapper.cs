namespace Quick.Mapper.Mapping;

/// <summary>
/// Interfaz principal para realizar mapeos entre objetos
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Mapea un objeto de origen a un nuevo objeto de destino
    /// </summary>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Mapea un objeto de origen a un nuevo objeto de destino con tipos genéricos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Mapea un objeto de origen a un objeto de destino existente
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <param name="source">Objeto de origen</param>
    /// <param name="destination">Objeto de destino existente</param>
    /// <returns>Objeto de destino con los datos mapeados</returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

    /// <summary>
    /// Mapea un objeto usando tipos en lugar de genéricos
    /// </summary>
    /// <param name="source">Objeto de origen</param>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Nuevo objeto de destino con los datos mapeados</returns>
    object Map(object source, Type sourceType, Type destinationType);

    /// <summary>
    /// Mapea un objeto a un objeto de destino existente usando tipos
    /// </summary>
    /// <param name="source">Objeto de origen</param>
    /// <param name="destination">Objeto de destino existente</param>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Objeto de destino con los datos mapeados</returns>
    object Map(object source, object destination, Type sourceType, Type destinationType);
}