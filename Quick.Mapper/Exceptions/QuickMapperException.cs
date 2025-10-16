namespace Quick.Mapper.Exceptions;

/// <summary>
/// Excepción base para todos los errores de Quick.Mapper
/// </summary>
public class QuickMapperException : Exception
{
    /// <summary>
    /// Crea una nueva instancia de QuickMapperException
    /// </summary>
    public QuickMapperException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de QuickMapperException con un mensaje
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    public QuickMapperException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de QuickMapperException con un mensaje y una excepción interna
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="innerException">Excepción interna</param>
    public QuickMapperException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}