using System.Linq.Expressions;
using System.Reflection;
using Quick.Mapper.Internal;

namespace Quick.Mapper.Mapping;

/// <summary>
/// Implementación no genérica de la expresión de mapeo
/// </summary>
public class MappingExpression : IMappingExpression
{
    protected readonly TypeMap TypeMap;

    /// <summary>
    /// Crea una nueva expresión de mapeo
    /// </summary>
    internal MappingExpression(TypeMap typeMap)
    {
        TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
    }

    /// <summary>
    /// Configura un mapeo para un miembro específico
    /// </summary>
    public IMappingExpression ForMember(string destinationMember, Action<IMemberConfigurationExpression> memberOptions)
    {
        var property = TypeMap.DestinationType.GetProperty(destinationMember);
        if (property == null)
        {
            throw new ArgumentException($"La propiedad '{destinationMember}' no existe en el tipo '{TypeMap.DestinationType.Name}'");
        }

        var propertyMap = TypeMap.GetOrCreatePropertyMap(property);
        var memberConfig = new MemberConfigurationExpression(propertyMap);
        memberOptions(memberConfig);

        return this;
    }

    /// <summary>
    /// Ejecuta una acción antes del mapeo
    /// </summary>
    public IMappingExpression BeforeMap(Action<object, object> beforeFunction)
    {
        var srcParam = Expression.Parameter(typeof(object), "src");
        var destParam = Expression.Parameter(typeof(object), "dest");
        var callExpr = Expression.Call(Expression.Constant(beforeFunction.Target), beforeFunction.Method, srcParam, destParam);
        TypeMap.BeforeMap = Expression.Lambda(callExpr, srcParam, destParam);
        return this;
    }

    /// <summary>
    /// Ejecuta una acción después del mapeo
    /// </summary>
    public IMappingExpression AfterMap(Action<object, object> afterFunction)
    {
        var srcParam = Expression.Parameter(typeof(object), "src");
        var destParam = Expression.Parameter(typeof(object), "dest");
        var callExpr = Expression.Call(Expression.Constant(afterFunction.Target), afterFunction.Method, srcParam, destParam);
        TypeMap.AfterMap = Expression.Lambda(callExpr, srcParam, destParam);
        return this;
    }

    /// <summary>
    /// Usa un constructor personalizado
    /// </summary>
    public IMappingExpression ConstructUsing(Func<object, object> constructor)
    {
        var param = Expression.Parameter(typeof(object), "src");
        var callExpr = Expression.Call(Expression.Constant(constructor.Target), constructor.Method, param);
        TypeMap.CustomConstructor = Expression.Lambda(callExpr, param);
        return this;
    }
}

/// <summary>
/// Implementación genérica de la expresión de mapeo
/// </summary>
public class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
{
    private readonly TypeMap _typeMap;
    private List<TypeMap>? _typeMaps; // Lista para agregar el ReverseMap

    /// <summary>
    /// Crea una nueva expresión de mapeo genérica
    /// </summary>
    internal MappingExpression(TypeMap typeMap)
    {
        _typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
    }

    /// <summary>
    /// Crea una nueva expresión de mapeo genérica con acceso a la lista de TypeMaps
    /// </summary>
    internal MappingExpression(TypeMap typeMap, List<TypeMap> typeMaps)
    {
        _typeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        _typeMaps = typeMaps;
    }

    /// <summary>
    /// Configura un mapeo para un miembro específico
    /// </summary>
    public IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
    {
        var memberExpression = destinationMember.Body as MemberExpression;
        if (memberExpression == null)
        {
            throw new ArgumentException("La expresión debe ser una propiedad", nameof(destinationMember));
        }

        var property = memberExpression.Member as PropertyInfo;
        if (property == null)
        {
            throw new ArgumentException("El miembro debe ser una propiedad", nameof(destinationMember));
        }

        var propertyMap = _typeMap.GetOrCreatePropertyMap(property);
        var memberConfig = new MemberConfigurationExpression<TSource, TDestination, TMember>(propertyMap);
        memberOptions(memberConfig);

        return this;
    }

    /// <summary>
    /// Ejecuta una acción antes del mapeo
    /// </summary>
    public IMappingExpression<TSource, TDestination> BeforeMap(Action<TSource, TDestination> beforeFunction)
    {
        var srcParam = Expression.Parameter(typeof(TSource), "src");
        var destParam = Expression.Parameter(typeof(TDestination), "dest");
        var callExpr = Expression.Call(Expression.Constant(beforeFunction.Target), beforeFunction.Method, srcParam, destParam);
        _typeMap.BeforeMap = Expression.Lambda(callExpr, srcParam, destParam);
        return this;
    }

    /// <summary>
    /// Ejecuta una acción después del mapeo
    /// </summary>
    public IMappingExpression<TSource, TDestination> AfterMap(Action<TSource, TDestination> afterFunction)
    {
        var srcParam = Expression.Parameter(typeof(TSource), "src");
        var destParam = Expression.Parameter(typeof(TDestination), "dest");
        var callExpr = Expression.Call(Expression.Constant(afterFunction.Target), afterFunction.Method, srcParam, destParam);
        _typeMap.AfterMap = Expression.Lambda(callExpr, srcParam, destParam);
        return this;
    }

    /// <summary>
    /// Usa un constructor personalizado
    /// </summary>
    public IMappingExpression<TSource, TDestination> ConstructUsing(Func<TSource, TDestination> constructor)
    {
        var param = Expression.Parameter(typeof(TSource), "src");
        var callExpr = Expression.Call(Expression.Constant(constructor.Target), constructor.Method, param);
        _typeMap.CustomConstructor = Expression.Lambda(callExpr, param);
        return this;
    }

    /// <summary>
    /// Realiza un mapeo inverso automático
    /// </summary>
    public IMappingExpression<TDestination, TSource> ReverseMap()
    {
        var reversePair = new TypePair(typeof(TDestination), typeof(TSource));
        var reverseTypeMap = new TypeMap(reversePair, typeof(TDestination), typeof(TSource));
        
        // Si tenemos acceso a la lista de TypeMaps, agregar el inverso
        if (_typeMaps != null)
        {
            _typeMaps.Add(reverseTypeMap);
            return new MappingExpression<TDestination, TSource>(reverseTypeMap, _typeMaps);
        }
        
        // Si no, retornar sin agregar (caso de uso directo sin Profile)
        return new MappingExpression<TDestination, TSource>(reverseTypeMap);
    }
}

/// <summary>
/// Configuración no genérica de un miembro
/// </summary>
internal class MemberConfigurationExpression : IMemberConfigurationExpression
{
    private readonly PropertyMap _propertyMap;

    public MemberConfigurationExpression(PropertyMap propertyMap)
    {
        _propertyMap = propertyMap;
    }

    public void MapFrom(Func<object, object> mapExpression)
    {
        var param = Expression.Parameter(typeof(object), "src");
        var callExpr = Expression.Call(Expression.Constant(mapExpression.Target), mapExpression.Method, param);
        _propertyMap.CustomMapper = Expression.Lambda(callExpr, param);
    }

    public void Ignore()
    {
        _propertyMap.IsIgnored = true;
    }

    public void Condition(Func<object, bool> condition)
    {
        var param = Expression.Parameter(typeof(object), "src");
        var callExpr = Expression.Call(Expression.Constant(condition.Target), condition.Method, param);
        _propertyMap.Condition = Expression.Lambda(callExpr, param);
    }
}

/// <summary>
/// Configuración genérica de un miembro
/// </summary>
internal class MemberConfigurationExpression<TSource, TDestination, TMember> 
    : IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    private readonly PropertyMap _propertyMap;

    public MemberConfigurationExpression(PropertyMap propertyMap)
    {
        _propertyMap = propertyMap;
    }

    public void MapFrom(Expression<Func<TSource, TMember>> mapExpression)
    {
        var param = Expression.Parameter(typeof(TSource), "src");
        var visitor = new ParameterReplacerVisitor(mapExpression.Parameters[0], param);
        var body = visitor.Visit(mapExpression.Body);
        _propertyMap.CustomMapper = Expression.Lambda(body, param);
    }

    public void MapFrom(Func<TSource, TMember> mapExpression)
    {
        var param = Expression.Parameter(typeof(TSource), "src");
        var callExpr = Expression.Call(Expression.Constant(mapExpression.Target), mapExpression.Method, param);
        _propertyMap.CustomMapper = Expression.Lambda(callExpr, param);
    }

    public void Ignore()
    {
        _propertyMap.IsIgnored = true;
    }

    public void Condition(Func<TSource, bool> condition)
    {
        var param = Expression.Parameter(typeof(TSource), "src");
        var callExpr = Expression.Call(Expression.Constant(condition.Target), condition.Method, param);
        _propertyMap.Condition = Expression.Lambda(callExpr, param);
    }

    public void UseValue(TMember value)
    {
        var param = Expression.Parameter(typeof(TSource), "src");
        var constant = Expression.Constant(value, typeof(TMember));
        _propertyMap.CustomMapper = Expression.Lambda(constant, param);
    }
}

/// <summary>
/// Visitante para reemplazar parámetros en expresiones
/// </summary>
internal class ParameterReplacerVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplacerVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}