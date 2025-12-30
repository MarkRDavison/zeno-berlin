namespace mark.davison.berlin.web.components.Pages;

[StateProperty<AuthorListState>]
[StateProperty<StoryListState>]
public partial class Author : StateComponent
{
    [Parameter]
    public required Guid Id { get; set; }

    [Inject]
    public required IDispatcher Dispatcher { get; set; }

    public AuthorDto Data => AuthorListState.Entities.FirstOrDefault(_ => _.AuthorId == Id) ?? new();

    private IEnumerable<StoryRowDto> _authorStories => StoryListState.Stories.Where(_ => _.Authors.Any(a => a == Id));

    protected override void OnParametersSet()
    {
        Dispatcher.Dispatch(new FetchStoryListAction
        {
            // TODO: Author id
        });
    }
}