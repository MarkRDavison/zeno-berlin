namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public class FetchDashboardListAction : BaseAction
{
    public int? Maximum { get; set; }
}

public class FetchDashboardListActionResponse : BaseActionResponse<List<DashboardTileDto>>
{

}
