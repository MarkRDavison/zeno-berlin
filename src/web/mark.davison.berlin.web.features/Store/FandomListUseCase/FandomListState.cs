namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

[FeatureState]
public sealed class FandomListState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<FandomDto> Entities { get; }

    public FandomListState() : this(false, Enumerable.Empty<FandomDto>())
    {

    }

    public FandomListState(bool isLoading, IEnumerable<FandomDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<FandomDto>(entities.ToList());
    }
}
