using System.Linq.Expressions;
using System.Reflection;

namespace Quick.Mapper.Internal;

/// <summary>
/// Representa el mapeo de una propiedad individual
/// </summary>
public class PropertyMap
{
    /// <summary>
    /// Propiedad de origen
    /// </summary>
    public PropertyInfo? SourceProperty { get; set; }

    /// <summary>
    /// Propiedad de destino
    /// </summary>
    public PropertyInfo DestinationProperty { get; }

    /// <summary>
    /// Función personalizada para mapear el valor
    /// </summary>
    public LambdaExpression? CustomMapper { get; set; }

    /// <summary>
    /// Indica si esta propiedad debe ser ignorada
    /// </summary>
    public bool IsIgnored { get; set; }

    /// <summary>
    /// Condición para mapear esta propiedad
    /// </summary>
    public LambdaExpression? Condition { get; set; }

    /// <summary>
    /// Indica si se debe mapear desde una función personalizada
    /// </summary>
    public bool HasCustomMapper => CustomMapper != null;

    /// <summary>
    /// Crea un nuevo mapeo de propiedad
    /// </summary>
    /// <param name="destinationProperty">Propiedad de destino</param>
    public PropertyMap(PropertyInfo destinationProperty)
    {
        DestinationProperty = destinationProperty ?? throw new ArgumentNullException(nameof(destinationProperty));
    }

    /// <summary>
    /// Obtiene el valor de la propiedad de origen desde un objeto
    /// </summary>
    public object? GetSourceValue(object source)
    {
        if (IsIgnored)
            return null;

        if (HasCustomMapper)
            return null; // El custom mapper se maneja en el MappingEngine

        if (SourceProperty == null)
            return null;

        return SourceProperty.GetValue(source);
    }

    /// <summary>
    /// Establece el valor en la propiedad de destino
    /// </summary>
    public void SetDestinationValue(object destination, object? value)
    {
        if (IsIgnored)
            return;

        if (DestinationProperty.CanWrite)
        {
            DestinationProperty.SetValue(destination, value);
        }
    }

    /// <summary>
    /// Valida que el mapeo de la propiedad es correcto
    /// </summary>
    public void Validate()
    {
        if (IsIgnored)
            return;

        if (!HasCustomMapper && SourceProperty == null)
        {
            throw new InvalidOperationException(
                $"La propiedad '{DestinationProperty.Name}' no tiene origen configurado y no está siendo ignorada.");
        }

        if (SourceProperty != null && !DestinationProperty.CanWrite)
        {
            throw new InvalidOperationException(
                $"La propiedad de destino '{DestinationProperty.Name}' no tiene setter público.");
        }
    }
}