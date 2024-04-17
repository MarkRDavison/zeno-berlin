namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public class AddStoryListAction : BaseAction
{
    public string StoryAddress { get; set; } = string.Empty;
}

public class AddStoryListActionResponse : BaseActionResponse<StoryDto>
{

}