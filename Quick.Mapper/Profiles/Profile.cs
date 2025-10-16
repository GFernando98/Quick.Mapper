using Quick.Mapper.Internal;
using Quick.Mapper.Mapping;

namespace Quick.Mapper.Profiles;

/// <summary>
/// Clase base para perfiles de mapeo que agrupan configuraciones relacionadas
/// </summary>
public abstract class Profile
{
    internal readonly List<TypeMap> TypeMaps = new();
    private InternalProfileExpression? _internalExpression;

    /// <summary>
    /// Nombre del perfil
    /// </summary>
    public string ProfileName { get; }

    /// <summary>
    /// Crea un nuevo perfil con el nombre de la clase
    /// </summary>
    protected Profile()
    {
        ProfileName = GetType().Name;
        _internalExpression = new InternalProfileExpression(this);
    }

    /// <summary>
    /// Crea un nuevo perfil con un nombre específico
    /// </summary>
    /// <param name="profileName">Nombre del perfil</param>
    protected Profile(string profileName)
    {
        ProfileName = profileName ?? throw new ArgumentNullException(nameof(profileName));
        _internalExpression = new InternalProfileExpression(this);
    }

    /// <summary>
    /// Crea un mapeo entre dos tipos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <returns>Expresión de mapeo para configuración adicional</returns>
    protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        if (_internalExpression == null)
        {
            throw new InvalidOperationException("La expresión interna no está inicializada");
        }

        return _internalExpression.CreateMap<TSource, TDestination>();
    }

    /// <summary>
    /// Crea un mapeo entre dos tipos usando Type en lugar de genéricos
    /// </summary>
    /// <param name="sourceType">Tipo de origen</param>
    /// <param name="destinationType">Tipo de destino</param>
    /// <returns>Expresión de mapeo no genérica</returns>
    protected IMappingExpression CreateMap(Type sourceType, Type destinationType)
    {
        if (_internalExpression == null)
        {
            throw new InvalidOperationException("La expresión interna no está inicializada");
        }

        return _internalExpression.CreateMap(sourceType, destinationType);
    }

    /// <summary>
    /// Representación en texto del perfil
    /// </summary>
    public override string ToString()
    {
        return ProfileName;
    }

    /// <summary>
    /// Expresión interna que acumula los TypeMaps del perfil
    /// </summary>
    private class InternalProfileExpression : IProfileExpression
    {
        private readonly Profile _profile;

        public InternalProfileExpression(Profile profile)
        {
            _profile = profile;
        }

        public IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var typePair = new TypePair(sourceType, destinationType);

            var typeMap = new TypeMap(typePair, sourceType, destinationType);
            _profile.TypeMaps.Add(typeMap);

            // Pasar la lista de TypeMaps para que ReverseMap funcione
            return new MappingExpression<TSource, TDestination>(typeMap, _profile.TypeMaps);
        }

        public IMappingExpression CreateMap(Type sourceType, Type destinationType)
        {
            var typePair = new TypePair(sourceType, destinationType);
            var typeMap = new TypeMap(typePair, sourceType, destinationType);
            _profile.TypeMaps.Add(typeMap);

            return new MappingExpression(typeMap);
        }
    }
}