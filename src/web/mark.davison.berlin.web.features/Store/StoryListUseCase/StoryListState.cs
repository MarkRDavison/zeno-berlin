namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public sealed class StoryListState : IClientState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<StoryRowDto> Stories { get; }
    public DateTime LastLoaded { get; }

    public StoryListState() : this(false, [], DateTime.MinValue)
    {

    }

    public StoryListState(bool isLoading, IEnumerable<StoryRowDto> stories, DateTime lastLoaded)
    {
        IsLoading = isLoading;
        Stories = new ReadOnlyCollection<StoryRowDto>(stories.ToList());
        LastLoaded = lastLoaded;
    }
}
