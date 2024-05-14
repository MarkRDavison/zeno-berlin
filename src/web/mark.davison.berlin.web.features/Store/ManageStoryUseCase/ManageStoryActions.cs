namespace mark.davison.berlin.web.features.Store.ManageStoryUseCase;

public sealed class FetchManageStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool SetLoading { get; set; }
}

public sealed class FetchManageStoryActionResponse : BaseActionResponse<StoryManageDto>
{

}

public sealed class SetFavouriteManageStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public sealed class SetFavouriteManageStoryActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}
public sealed class SetManageStoryConsumedChaptersAction : BaseAction
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}
public sealed class SetManageStoryConsumedChaptersActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}

public sealed class UpdateManageStoryActionResponse : BaseActionResponse<StoryRowDto>
{
    public Guid StoryId { get; set; }
}

public sealed class AddManageStoryUpdateAction : BaseAction // TODO: Source gen to create this from command/query???
{
    public Guid StoryId { get; set; }
    public int CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public bool Complete { get; set; }
    public DateOnly UpdateDate { get; set; }
}

public sealed class AddManageStoryUpdateActionResponse : BaseActionResponse<StoryUpdateDto>
{

}