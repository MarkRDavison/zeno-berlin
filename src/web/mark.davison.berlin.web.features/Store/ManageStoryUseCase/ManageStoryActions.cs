namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public class FetchManageStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool SetLoading { get; set; }
}

public class FetchManageStoryActionResponse : BaseActionResponse<StoryManageDto>
{

}

public class SetFavouriteManageStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class SetFavouriteManageStoryActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class UpdateManageStoryActionResponse : BaseActionResponse<StoryRowDto>
{
    public Guid StoryId { get; set; }
}