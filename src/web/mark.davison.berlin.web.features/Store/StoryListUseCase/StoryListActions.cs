namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public class FetchStoryListAction : BaseAction
{
    // TODO: Query params???
}

public class FetchStoryListActionResponse : BaseActionResponse<List<StoryDto>>
{

}

public class AddStoryListAction : BaseAction
{
    public string StoryAddress { get; set; } = string.Empty;
}

public class AddStoryListActionResponse : BaseActionResponse<StoryDto>
{

}

public class DeleteStoryListAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class DeleteStoryListActionResponse : BaseActionResponse<DeleteStoryCommandResponse>
{

}

public class SetFavouriteStoryListAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class SetFavouriteStoryListActionResponse : BaseActionResponse<StoryDto>
{

}

public class UpdateStoryListAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class UpdateStoryListActionResponse : BaseActionResponse<StoryDto>
{

}