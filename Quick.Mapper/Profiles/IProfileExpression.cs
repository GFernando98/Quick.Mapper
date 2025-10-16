using Quick.Mapper.Mapping;

namespace Quick.Mapper.Profiles;

/// <summary>
/// Interfaz para expresiones de configuración de perfiles
/// </summary>
public interface IProfileExpression
{
    /// <summary>
    /// Crea un mapeo entre dos tipos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <returns>Expresión de mapeo para configuración adicional</returns>
    IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();

    /// <summary>
    /// Crea un mapeo entre dos tipos usando Type en lugar de genéricos
    /// </summary>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Expresión de mapeo no genérica</returns>
    IMappingExpression CreateMap(Type sourceType, Type destinationType);
}