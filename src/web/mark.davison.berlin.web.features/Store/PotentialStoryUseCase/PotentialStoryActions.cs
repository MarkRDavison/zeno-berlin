namespace mark.davison.berlin.web.features.Store.PotentialStoryUseCase;

public sealed class FetchPotentialStoriesAction : BaseAction
{

}

public sealed class FetchPotentialStoriesActionResponse : BaseActionResponse<List<PotentialStoryDto>>
{

}

public sealed class AddPotentialStoryAction : BaseAction
{
    public string StoryAddress { get; set; } = string.Empty;
}

public sealed class AddPotentialStoryActionResponse : BaseActionResponse<PotentialStoryDto>
{

}

public sealed class DeletePotentialStoryAction : BaseAction
{
    public Guid Id { get; set; }
}

public sealed class DeletePotentialStoryActionResponse : BaseActionResponse<Guid>
{

}

public sealed class GrabPotentialStoryAction : BaseAction
{
    public Guid Id { get; set; }
}

public sealed class GrabPotentialStoryActionResponse : BaseActionResponse<StoryDto>
{

}
