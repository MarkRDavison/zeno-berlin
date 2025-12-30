namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public sealed class AuthorListState : IClientState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<AuthorDto> Entities { get; }

    public AuthorListState() : this(false, Enumerable.Empty<AuthorDto>())
    {

    }

    public AuthorListState(bool isLoading, IEnumerable<AuthorDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<AuthorDto>([.. entities]);
    }
}
