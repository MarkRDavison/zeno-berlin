namespace mark.davison.berlin.web.features.Store.SharedStoryUseCase;

public sealed class SetStoryConsumedChaptersAction : BaseAction
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}
public sealed class SetStoryConsumedChaptersActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public int? ConsumedChapters { get; set; }
}

public sealed class SetStoryFavouriteAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public sealed class SetStoryFavouriteActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public sealed class DeleteStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public sealed class DeleteStoryActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
}

public sealed class UpdateStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public sealed class UpdateStoryActionResponse : BaseActionResponse<StoryRowDto>
{
    public Guid StoryId { get; set; }
}
public sealed class AddStoryAction : BaseAction
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? UpdateTypeId { get; set; }
}

public sealed class AddStoryActionResponse : BaseActionResponse<StoryDto>
{

}