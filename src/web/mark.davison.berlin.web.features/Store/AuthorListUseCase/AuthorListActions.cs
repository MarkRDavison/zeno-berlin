namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public class FetchAuthorsListAction : BaseAction
{
    public List<Guid> AuthorIds { get; set; } = [];
}

public class FetchAuthorsListActionResponse : BaseActionResponse<List<AuthorDto>>
{

}