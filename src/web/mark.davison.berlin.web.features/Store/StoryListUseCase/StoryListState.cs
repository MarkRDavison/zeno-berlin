namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

[FeatureState]
public class StoryListState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<StoryDto> Stories { get; }

    public StoryListState() : this(false, Enumerable.Empty<StoryDto>())
    {

    }

    public StoryListState(bool isLoading, IEnumerable<StoryDto> stories)
    {
        IsLoading = isLoading;
        Stories = new ReadOnlyCollection<StoryDto>(stories.ToList());
    }
}
