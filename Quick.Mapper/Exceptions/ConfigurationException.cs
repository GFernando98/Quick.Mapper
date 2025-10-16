namespace Quick.Mapper.Exceptions;

/// <summary>
/// Excepción lanzada cuando hay un error en la configuración del mapper
/// </summary>
public class ConfigurationException : QuickMapperException
{
    /// <summary>
    /// Crea una nueva instancia de ConfigurationException
    /// </summary>
    public ConfigurationException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de ConfigurationException con un mensaje
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    public ConfigurationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de ConfigurationException con un mensaje y una excepción interna
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="innerException">Excepción interna</param>
    public ConfigurationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}