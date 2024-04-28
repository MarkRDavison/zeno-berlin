namespace mark.davison.berlin.web.components.Pages;
public partial class Author
{
    [Parameter]
    public required Guid Id { get; set; }

    public AuthorDto Data => AuthorListState.Value.Entities.FirstOrDefault(_ => _.AuthorId == Id) ?? new();

    [Inject]
    public required IState<AuthorListState> AuthorListState { get; set; }
}
