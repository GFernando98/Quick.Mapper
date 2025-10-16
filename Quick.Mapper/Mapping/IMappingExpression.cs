using System.Linq.Expressions;

namespace Quick.Mapper.Mapping;

/// <summary>
/// Interfaz no genérica para expresiones de mapeo
/// </summary>
public interface IMappingExpression
{
    /// <summary>
    /// Configura un mapeo para un miembro específico
    /// </summary>
    /// <param name="destinationMember">Nombre del miembro de destino</param>
    /// <param name="memberOptions">Opciones de configuración del miembro</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression ForMember(string destinationMember, Action<IMemberConfigurationExpression> memberOptions);

    /// <summary>
    /// Ejecuta una acción antes del mapeo
    /// </summary>
    /// <param name="beforeFunction">Acción a ejecutar</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression BeforeMap(Action<object, object> beforeFunction);

    /// <summary>
    /// Ejecuta una acción después del mapeo
    /// </summary>
    /// <param name="afterFunction">Acción a ejecutar</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression AfterMap(Action<object, object> afterFunction);

    /// <summary>
    /// Usa un constructor personalizado para crear instancias del objeto de destino
    /// </summary>
    /// <param name="constructor">Función que crea la instancia</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression ConstructUsing(Func<object, object> constructor);
}

/// <summary>
/// Interfaz genérica para expresiones de mapeo con sintaxis fluida
/// </summary>
/// <typeparam name="TSource">Tipo de origen</typeparam>
/// <typeparam name="TDestination">Tipo de destino</typeparam>
public interface IMappingExpression<TSource, TDestination>
{
    /// <summary>
    /// Configura un mapeo para un miembro específico del objeto de destino
    /// </summary>
    /// <typeparam name="TMember">Tipo del miembro</typeparam>
    /// <param name="destinationMember">Expresión que selecciona el miembro de destino</param>
    /// <param name="memberOptions">Opciones de configuración del miembro</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);

    /// <summary>
    /// Ejecuta una acción antes del mapeo
    /// </summary>
    /// <param name="beforeFunction">Acción a ejecutar</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression<TSource, TDestination> BeforeMap(Action<TSource, TDestination> beforeFunction);

    /// <summary>
    /// Ejecuta una acción después del mapeo
    /// </summary>
    /// <param name="afterFunction">Acción a ejecutar</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression<TSource, TDestination> AfterMap(Action<TSource, TDestination> afterFunction);

    /// <summary>
    /// Usa un constructor personalizado para crear instancias del objeto de destino
    /// </summary>
    /// <param name="constructor">Función que crea la instancia</param>
    /// <returns>Esta expresión de mapeo para encadenamiento</returns>
    IMappingExpression<TSource, TDestination> ConstructUsing(Func<TSource, TDestination> constructor);

    /// <summary>
    /// Realiza un mapeo inverso automático (TDestination -> TSource)
    /// </summary>
    /// <returns>Expresión de mapeo inverso</returns>
    IMappingExpression<TDestination, TSource> ReverseMap();
}

/// <summary>
/// Interfaz para configurar opciones de un miembro específico (no genérica)
/// </summary>
public interface IMemberConfigurationExpression
{
    /// <summary>
    /// Mapea desde una función personalizada
    /// </summary>
    /// <param name="mapExpression">Función que obtiene el valor</param>
    void MapFrom(Func<object, object> mapExpression);

    /// <summary>
    /// Ignora este miembro durante el mapeo
    /// </summary>
    void Ignore();

    /// <summary>
    /// Mapea solo si se cumple una condición
    /// </summary>
    /// <param name="condition">Condición a evaluar</param>
    void Condition(Func<object, bool> condition);
}

/// <summary>
/// Interfaz genérica para configurar opciones de un miembro específico
/// </summary>
/// <typeparam name="TSource">Tipo de origen</typeparam>
/// <typeparam name="TDestination">Tipo de destino</typeparam>
/// <typeparam name="TMember">Tipo del miembro</typeparam>
public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    /// <summary>
    /// Mapea desde una propiedad o expresión del objeto de origen
    /// </summary>
    /// <param name="mapExpression">Expresión que obtiene el valor desde el origen</param>
    void MapFrom(Expression<Func<TSource, TMember>> mapExpression);

    /// <summary>
    /// Mapea desde una función que recibe el objeto completo de origen
    /// </summary>
    /// <param name="mapExpression">Función que obtiene el valor</param>
    void MapFrom(Func<TSource, TMember> mapExpression);

    /// <summary>
    /// Ignora este miembro durante el mapeo
    /// </summary>
    void Ignore();

    /// <summary>
    /// Mapea solo si se cumple una condición
    /// </summary>
    /// <param name="condition">Condición a evaluar</param>
    void Condition(Func<TSource, bool> condition);

    /// <summary>
    /// Usa un valor constante para este miembro
    /// </summary>
    /// <param name="value">Valor constante</param>
    void UseValue(TMember value);
}