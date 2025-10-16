namespace Quick.Mapper.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se encuentra un mapeo registrado entre dos tipos
/// </summary>
public class MappingNotFoundException : QuickMapperException
{
    /// <summary>
    /// Tipo de origen del mapeo
    /// </summary>
    public Type? SourceType { get; }

    /// <summary>
    /// Tipo de destino del mapeo
    /// </summary>
    public Type? DestinationType { get; }

    /// <summary>
    /// Crea una nueva instancia de MappingNotFoundException
    /// </summary>
    public MappingNotFoundException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de MappingNotFoundException con un mensaje
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    public MappingNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de MappingNotFoundException con los tipos involucrados
    /// </summary>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    public MappingNotFoundException(Type sourceType, Type destinationType)
        : base($"No se encontró un mapeo configurado de '{sourceType.Name}' a '{destinationType.Name}'. " +
               $"Asegúrate de llamar a CreateMap<{sourceType.Name}, {destinationType.Name}>() en la configuración.")
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }

    /// <summary>
    /// Crea una nueva instancia de MappingNotFoundException con un mensaje y una excepción interna
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="innerException">Excepción interna</param>
    public MappingNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de MappingNotFoundException con tipos y excepción interna
    /// </summary>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <param name="innerException">Excepción interna</param>
    public MappingNotFoundException(Type sourceType, Type destinationType, Exception innerException)
        : base($"No se encontró un mapeo configurado de '{sourceType.Name}' a '{destinationType.Name}'.", innerException)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}