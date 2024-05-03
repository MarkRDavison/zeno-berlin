namespace mark.davison.berlin.shared.validation.Context;

public interface IValidationContext
{
    Task<T?> GetById<T>(Guid id, CancellationToken cancellationToken) where T : BerlinEntity, new();
    Task<T> GetByIdValidated<T>(Guid id, CancellationToken cancellationToken) where T : BerlinEntity, new();

    Task<T?> GetByProperty<T>(Expression<Func<T, bool>> predicate, string name, CancellationToken cancellationToken)
        where T : BerlinEntity, new();

    Task<List<T>> GetAll<T>(CancellationToken cancellationToken) where T : BerlinEntity, new();

    Task<List<T>> GetAllForUserId<T>(Guid userId, CancellationToken cancellationToken) where T : BerlinEntity, new();
}