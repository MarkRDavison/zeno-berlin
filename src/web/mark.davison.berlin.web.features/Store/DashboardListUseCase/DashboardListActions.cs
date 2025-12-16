namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public sealed class FetchDashboardListAction : BaseAction
{
    public int? Maximum { get; set; }
}

public sealed class FetchDashboardListActionResponse : BaseActionResponse<List<DashboardTileDto>>
{

}

public sealed class SetFavouriteDashboardListAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public sealed class SetFavouriteDashboardListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}