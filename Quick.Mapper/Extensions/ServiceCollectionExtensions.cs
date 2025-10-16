using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quick.Mapper.Configuration;
using Quick.Mapper.Mapping;
using Quick.Mapper.Profiles;

namespace Quick.Mapper.Extensions;

/// <summary>
/// Extensiones para registrar Quick.Mapper en el contenedor de dependencias
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Agrega Quick.Mapper al contenedor de servicios con configuración personalizada
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configAction">Acción para configurar los mapeos</param>
    /// <returns>Colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddQuickMapper(
        this IServiceCollection services,
        Action<MapperConfigurationExpression> configAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configAction);

        // Crear la configuración
        var configuration = new MapperConfiguration(configAction);

        // Registrar la configuración como singleton
        services.TryAddSingleton<IMapperConfiguration>(configuration);
        services.TryAddSingleton(configuration);

        // Registrar el mapper como singleton
        services.TryAddSingleton<IMapper>(sp => configuration.CreateMapper());

        return services;
    }

    /// <summary>
    /// Agrega Quick.Mapper al contenedor de servicios escaneando perfiles en ensamblados
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="assemblies">Ensamblados donde buscar perfiles</param>
    /// <returns>Colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddQuickMapper(
        this IServiceCollection services,
        params Assembly[]? assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        return AddQuickMapper(services, cfg =>
        {
            if (assemblies != null && assemblies.Length > 0)
            {
                cfg.AddProfiles(assemblies);
            }
        });
    }

    /// <summary>
    /// Agrega Quick.Mapper al contenedor de servicios escaneando perfiles por tipos
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="profileTypes">Tipos de perfiles a agregar</param>
    /// <returns>Colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddQuickMapper(
        this IServiceCollection services,
        params Type[] profileTypes)
    {
        ArgumentNullException.ThrowIfNull(services);

        return AddQuickMapper(services, cfg =>
        {
            if (profileTypes != null && profileTypes.Length > 0)
            {
                cfg.AddProfiles(profileTypes);
            }
        });
    }

    /// <summary>
    /// Agrega Quick.Mapper al contenedor de servicios usando un tipo marcador del ensamblado
    /// </summary>
    /// <typeparam name="TMarker">Tipo marcador del ensamblado</typeparam>
    /// <param name="services">Colección de servicios</param>
    /// <returns>Colección de servicios para encadenamiento</returns>
    public static IServiceCollection AddQuickMapper<TMarker>(this IServiceCollection services)
    {
        return AddQuickMapper(services, typeof(TMarker).Assembly);
    }

    /// <summary>
    /// Agrega perfiles desde ensamblados a la expresión de configuración
    /// </summary>
    private static void AddProfiles(this MapperConfigurationExpression expression, 
        params System.Reflection.Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var profileTypes = assembly.GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t) && 
                           !t.IsAbstract && 
                           !t.IsInterface)
                .ToList();

            foreach (var profileType in profileTypes)
            {
                expression.AddProfile(profileType);
            }
        }
    }
}