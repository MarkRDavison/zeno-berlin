namespace mark.davison.berlin.web.features.Store.SharedStoryUseCase;

public class SetStoryFavouriteAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class SetStoryFavouriteActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class DeleteStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class DeleteStoryActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
}

public class UpdateStoryAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class UpdateStoryActionResponse : BaseActionResponse<StoryRowDto>
{
    public Guid StoryId { get; set; }
}
public class AddStoryAction : BaseAction
{
    public string StoryAddress { get; set; } = string.Empty;
    public Guid? UpdateTypeId { get; set; }
}

public class AddStoryActionResponse : BaseActionResponse<StoryDto>
{

}