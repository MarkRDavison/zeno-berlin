namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public sealed class FetchStoryListAction : BaseAction
{
    // TODO: Query params???
}

public sealed class FetchStoryListActionResponse : BaseActionResponse<List<StoryRowDto>>
{

}

public sealed class DeleteStoryListAction : BaseAction
{
    public Guid StoryId { get; set; }
}

public sealed class DeleteStoryListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
}

public sealed class SetFavouriteStoryListAction : BaseAction // TODO: Eagerly updating ui, good for like clicking icons, but excessive repetition, maybe a way of handling?? Keep reducer but use shared action/response???
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public sealed class SetFavouriteStoryListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}