namespace Quick.Mapper.Internal;

/// <summary>
/// Representa un par de tipos (origen y destino) para identificar mapeos
/// </summary>
public readonly struct TypePair : IEquatable<TypePair>
{
    /// <summary>
    /// Tipo de origen
    /// </summary>
    public Type SourceType { get; }

    /// <summary>
    /// Tipo de destino
    /// </summary>
    public Type DestinationType { get; }

    /// <summary>
    /// Crea un nuevo par de tipos
    /// </summary>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    public TypePair(Type sourceType, Type destinationType)
    {
        SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
    }

    /// <summary>
    /// Determina si dos TypePair son iguales
    /// </summary>
    public bool Equals(TypePair other)
    {
        return SourceType == other.SourceType && DestinationType == other.DestinationType;
    }

    /// <summary>
    /// Determina si un objeto es igual a este TypePair
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is TypePair other && Equals(other);
    }

    /// <summary>
    /// Obtiene el código hash de este TypePair
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(SourceType, DestinationType);
    }

    /// <summary>
    /// Representación en texto del par de tipos
    /// </summary>
    public override string ToString()
    {
        return $"{SourceType.Name} -> {DestinationType.Name}";
    }

    public static bool operator ==(TypePair left, TypePair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TypePair left, TypePair right)
    {
        return !left.Equals(right);
    }
}