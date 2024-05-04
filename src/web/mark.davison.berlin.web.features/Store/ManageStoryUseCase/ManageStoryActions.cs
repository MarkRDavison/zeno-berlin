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
public class SetManageStoryConsumedChaptersAction : BaseAction
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}
public class SetManageStoryConsumedChaptersActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}

public class UpdateManageStoryActionResponse : BaseActionResponse<StoryRowDto>
{
    public Guid StoryId { get; set; }
}

public class AddManageStoryUpdateAction : BaseAction // TODO: Source gen to create this from command/query???
{
    public Guid StoryId { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly UpdateDate { get; set; }
}

public class AddManageStoryUpdateActionResponse : BaseActionResponse<StoryUpdateDto>
{

}