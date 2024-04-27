namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public class FetchStoryListAction : BaseAction
{
    // TODO: Query params???
}

public class FetchStoryListActionResponse : BaseActionResponse<List<StoryRowDto>>
{

}

public class DeleteStoryListAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public class DeleteStoryListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
}

public class SetFavouriteStoryListAction : BaseAction // TODO: Eagerly updating ui, good for like clicking icons, but excessive repetition, maybe a way of handling?? Keep reducer but use shared action/response???
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class SetFavouriteStoryListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}