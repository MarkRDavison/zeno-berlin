namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

public class FetchFandomsListAction : BaseAction
{
}

public class FetchFandomsListActionResponse : BaseActionResponse<List<FandomDto>>
{

}

public class EditFandomListAction : BaseAction
{
    public Guid FandomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid? ParentFandomId { get; set; }
}

public class EditFandomListActionResponse : BaseActionResponse<FandomDto>
{

}

public class AddFandomListAction : BaseAction
{
    public string Name { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public Guid? ParentFandomId { get; set; }
}

public class AddFandomListActionResponse : BaseActionResponse<FandomDto>
{

}