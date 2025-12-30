namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

public sealed class FandomListState : IClientState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<FandomDto> Entities { get; }

    public FandomListState() : this(false, [])
    {

    }

    public FandomListState(bool isLoading, IEnumerable<FandomDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<FandomDto>([.. entities]);
    }
}
