namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

public sealed class PotentialStoryState : IClientState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<PotentialStoryDto> Entities { get; }

    public PotentialStoryState() : this(false, [])
    {

    }

    public PotentialStoryState(bool isLoading, IEnumerable<PotentialStoryDto> entities)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<PotentialStoryDto>([.. entities]);
    }
}