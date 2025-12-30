namespace mark.davison.berlin.web.components.Pages;

[StateProperty<AuthorListState>]
public partial class AuthorsPage : StateComponent
{
    [Inject]
    public required IStoreHelper StoreHelper { get; set; }

    private IEnumerable<AuthorDto> _authors => AuthorListState.Entities.OrderBy(_ => _.Name);

    protected override async Task OnInitializedAsync()
    {
        await StoreHelper.DispatchAndWaitForResponse<FetchAuthorsListAction, FetchAuthorsListActionResponse>(new FetchAuthorsListAction());
    }

    private string? GetAuthorName(Guid authorId)
    {
        return AuthorListState.Entities.FirstOrDefault(_ => _.AuthorId == authorId)?.Name;
    }
}
