namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public class FetchDashboardListAction : BaseAction
{
    public int? Maximum { get; set; }
}

public class FetchDashboardListActionResponse : BaseActionResponse<List<DashboardTileDto>>
{

}

public class SetFavouriteDashboardListAction : BaseAction
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}

public class SetFavouriteDashboardListActionResponse : BaseActionResponse
{
    public Guid StoryId { get; set; }
    public bool IsFavourite { get; set; }
}