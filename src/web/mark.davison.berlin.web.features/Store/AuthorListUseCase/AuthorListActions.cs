namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public sealed class FetchAuthorsListAction : BaseAction
{
    public List<Guid> AuthorIds { get; set; } = [];
}

public sealed class FetchAuthorsListActionResponse : BaseActionResponse<List<AuthorDto>>
{

}