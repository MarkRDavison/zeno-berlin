namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

[FeatureState]
public class AuthorListState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<AuthorDto> Entities { get; }

    public AuthorListState() : this(false, Enumerable.Empty<AuthorDto>())
    {

    }

    public AuthorListState(bool isLoading, IEnumerable<AuthorDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<AuthorDto>(entities.ToList());
    }
}
