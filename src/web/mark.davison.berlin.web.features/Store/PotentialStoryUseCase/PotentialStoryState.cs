namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

[FeatureState]
public class PotentialStoryState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<PotentialStoryDto> Entities { get; }

    public PotentialStoryState() : this(false, Enumerable.Empty<PotentialStoryDto>())
    {

    }

    public PotentialStoryState(bool isLoading, IEnumerable<PotentialStoryDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<PotentialStoryDto>(entities.ToList());
    }
}
