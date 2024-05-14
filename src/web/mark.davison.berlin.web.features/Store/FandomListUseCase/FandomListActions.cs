namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

public sealed class FetchFandomsListAction : BaseAction
{
}

public sealed class FetchFandomsListActionResponse : BaseActionResponse<List<FandomDto>>
{

}

public sealed class EditFandomListAction : BaseAction
{
    public Guid FandomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid? ParentFandomId { get; set; }
}

public sealed class EditFandomListActionResponse : BaseActionResponse<FandomDto>
{

}

public sealed class AddFandomListAction : BaseAction
{
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid? ParentFandomId { get; set; }
}

public sealed class AddFandomListActionResponse : BaseActionResponse<FandomDto>
{

}