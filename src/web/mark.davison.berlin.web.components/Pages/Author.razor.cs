namespace mark.davison.berlin.web.components.Pages;

public partial class Author
{
    [Parameter]
    public required Guid Id { get; set; }


    public AuthorDto Data => AuthorListState.Value.Entities.FirstOrDefault(_ => _.AuthorId == Id) ?? new();

    [Inject]
    public required IState<AuthorListState> AuthorListState { get; set; }

    [Inject]
    public required IState<StoryListState> StoryListState { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    private IEnumerable<StoryRowDto> _authorStories => StoryListState.Value.Stories.Where(_ => _.Authors.Any(a => a == Id));

    protected override void OnParametersSet()
    {
        Dispatcher.Dispatch(new FetchStoryListAction
        {
            // TODO: Author id
        });
    }
}
