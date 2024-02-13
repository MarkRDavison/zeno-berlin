using System.Linq.Expressions;

namespace mark.davison.berlin.shared.validation.Context;

public interface IValidationContext
{
    Task<T?> GetById<T>(Guid id, CancellationToken cancellationToken) where T : BerlinEntity, new();

    Task<T?> GetByProperty<T, TProp>(Expression<Func<T, bool>> predicate, string name, CancellationToken cancellationToken)
        where T : BerlinEntity, new();

    Task<List<T>> GetAll<T>(CancellationToken cancellationToken) where T : BerlinEntity, new();
}