namespace mark.davison.berlin.web.components.Pages;

public partial class AuthorsPage
{
    [Inject]
    public required IState<AuthorListState> AuthorListState { get; set; }

    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    private IEnumerable<AuthorDto> _authors => AuthorListState.Value.Entities.OrderBy(_ => _.Name);

    protected override async Task OnInitializedAsync()
    {
        await StoreHelper.DispatchAndWaitForResponse<FetchAuthorsListAction, FetchAuthorsListActionResponse>(new FetchAuthorsListAction());
    }

    private string? GetAuthorName(Guid authorId)
    {
        return AuthorListState.Value.Entities.FirstOrDefault(_ => _.AuthorId == authorId)?.Name;
    }
}
