namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public class FetchManageStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class FetchManageStoryActionResponse : BaseActionResponse<StoryManageDto>
{

}