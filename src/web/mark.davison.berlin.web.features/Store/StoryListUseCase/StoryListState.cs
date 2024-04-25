namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

[FeatureState]
public class StoryListState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<StoryDto> Stories { get; }
    public DateTime LastLoaded { get; }

    public StoryListState() : this(false, Enumerable.Empty<StoryDto>(), DateTime.MinValue)
    {

    }

    public StoryListState(bool isLoading, IEnumerable<StoryDto> stories, DateTime lastLoaded)
    {
        IsLoading = isLoading;
        Stories = new ReadOnlyCollection<StoryDto>(stories.ToList());
        LastLoaded = lastLoaded;
    }
}
