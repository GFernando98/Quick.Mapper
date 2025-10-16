using Quick.Mapper.Internal;
using Quick.Mapper.Mapping;

namespace Quick.Mapper.Configuration;

/// <summary>
/// Interfaz principal de configuración del mapper
/// </summary>
public interface IMapperConfiguration
{
    /// <summary>
    /// Crea una instancia del mapper basada en esta configuración
    /// </summary>
    /// <returns>Instancia de IMapper lista para usar</returns>
    IMapper CreateMapper();

    /// <summary>
    /// Valida que todas las configuraciones de mapeo son correctas
    /// </summary>
    void AssertConfigurationIsValid();

    /// <summary>
    /// Obtiene todos los mapeos de tipos registrados
    /// </summary>
    IEnumerable<TypeMap> GetAllTypeMaps();
}