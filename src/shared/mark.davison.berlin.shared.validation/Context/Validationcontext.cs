namespace mark.davison.berlin.shared.validation.Context;

public class ValidationContext : IValidationContext
{
    private readonly IReadonlyRepository _repository;

    private readonly IDictionary<Type, IDictionary<Guid, BerlinEntity?>> _idCache;
    private readonly IDictionary<(Type, string), BerlinEntity?> _propertyCache;
    private readonly IDictionary<Type, List<BerlinEntity>> _allCache;

    public ValidationContext(
        IReadonlyRepository repository
    )
    {
        _repository = repository;

        _idCache = new Dictionary<Type, IDictionary<Guid, BerlinEntity?>>();
        _allCache = new Dictionary<Type, List<BerlinEntity>>();
        _propertyCache = new Dictionary<(Type, string), BerlinEntity?>();
    }

    public async Task<T?> GetById<T>(Guid id, CancellationToken cancellationToken)
        where T : BerlinEntity, new()
    {
        IDictionary<Guid, BerlinEntity?>? entityCache;

        if (!_idCache.TryGetValue(typeof(T), out entityCache))
        {
            entityCache = new Dictionary<Guid, BerlinEntity?>();
        }

        T? entity;

        if (entityCache.TryGetValue(id, out var berlinEntity))
        {
            entity = berlinEntity as T;
        }
        else
        {
            entity = await _repository.GetEntityAsync<T>(id, cancellationToken);
            entityCache.Add(id, entity);
        }

        return entity;
    }

    public async Task<T?> GetByProperty<T, TProp>(Expression<Func<T, bool>> predicate, string name, CancellationToken cancellationToken)
        where T : BerlinEntity, new()
    {
        T? entity;

        if (_propertyCache.TryGetValue((typeof(T), name), out var berlinEntity))
        {
            entity = berlinEntity as T;
        }
        else
        {
            entity = await _repository.GetEntityAsync<T>(predicate, cancellationToken);
            _propertyCache.Add((typeof(T), name), entity);
        }

        return entity;
    }

    public async Task<List<T>> GetAll<T>(CancellationToken cancellationToken) where T : BerlinEntity, new()
    {
        if (_allCache.ContainsKey(typeof(T)))
        {
            return _allCache[typeof(T)].Cast<T>().ToList();
        }

        var all = await _repository.GetEntitiesAsync<T>(cancellationToken);
        _allCache[typeof(T)] = [.. all];
        return all;
    }

    public static PropertyInfo GetPropertyInfo<T, TProperty>(
        Expression<Func<T, TProperty>> propertyLambda
    )
    {
        if (propertyLambda.Body is not MemberExpression member)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                propertyLambda.ToString()));
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                propertyLambda.ToString()));
        }

        Type type = typeof(T);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a property that is not from type {1}.",
                propertyLambda.ToString(),
                type));
        }

        return propInfo;
    }
}
