using Quick.Mapper.Internal;
using Quick.Mapper.Mapping;
using Quick.Mapper.Profiles;

namespace Quick.Mapper.Configuration;

/// <summary>
/// Expresión de configuración que permite definir mapeos
/// </summary>
public class MapperConfigurationExpression : IProfileExpression
{
    internal readonly List<TypeMap> TypeMaps = new();
    internal readonly List<Profile> Profiles = new();

    /// <summary>
    /// Crea un mapeo entre dos tipos
    /// </summary>
    /// <typeparam name="TSource">Tipo de origen</typeparam>
    /// <typeparam name="TDestination">Tipo de destino</typeparam>
    /// <returns>Expresión de mapeo para configuración adicional</returns>
    public IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var sourceType = typeof(TSource);
        var destinationType = typeof(TDestination);
        var typePair = new TypePair(sourceType, destinationType);
        
        var typeMap = new TypeMap(typePair, sourceType, destinationType);
        TypeMaps.Add(typeMap);
        
        // Pasar la lista de TypeMaps para que ReverseMap funcione
        return new MappingExpression<TSource, TDestination>(typeMap, TypeMaps);
    }

    /// <summary>
    /// Crea un mapeo entre dos tipos usando Type en lugar de genéricos
    /// </summary>
    public IMappingExpression CreateMap(Type sourceType, Type destinationType)
    {
        var typePair = new TypePair(sourceType, destinationType);
        var typeMap = new TypeMap(typePair, sourceType, destinationType);
        TypeMaps.Add(typeMap);
        
        return new MappingExpression(typeMap);
    }

    /// <summary>
    /// Agrega un perfil de mapeo
    /// </summary>
    /// <typeparam name="TProfile">Tipo del perfil</typeparam>
    public void AddProfile<TProfile>() where TProfile : Profile, new()
    {
        var profile = new TProfile();
        AddProfile(profile);
    }

    /// <summary>
    /// Agrega una instancia de perfil de mapeo
    /// </summary>
    public void AddProfile(Profile profile)
    {
        Profiles.Add(profile);
        
        // Agregar todos los TypeMaps del perfil a la configuración
        foreach (var typeMap in profile.TypeMaps)
        {
            TypeMaps.Add(typeMap);
        }
    }

    /// <summary>
    /// Agrega un perfil de mapeo por tipo
    /// </summary>
    public void AddProfile(Type profileType)
    {
        if (!typeof(Profile).IsAssignableFrom(profileType))
        {
            throw new ArgumentException($"El tipo {profileType.Name} no hereda de Profile", nameof(profileType));
        }

        var profile = (Profile)Activator.CreateInstance(profileType)!;
        AddProfile(profile);
    }

    /// <summary>
    /// Agrega múltiples perfiles desde un ensamblado
    /// </summary>
    public void AddProfiles(params Type[] profileTypes)
    {
        foreach (var profileType in profileTypes)
        {
            AddProfile(profileType);
        }
    }
}