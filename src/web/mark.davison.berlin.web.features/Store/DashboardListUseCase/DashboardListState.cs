namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

public sealed class DashboardListState : IClientState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<DashboardTileDto> Entities { get; }
    public DateTime LastLoaded { get; }

    public DashboardListState() : this(false, [], DateTime.MinValue)
    {

    }

    public DashboardListState(bool isLoading, IEnumerable<DashboardTileDto> stories, DateTime lastLoaded)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<DashboardTileDto>(stories.ToList());
        LastLoaded = lastLoaded;
    }
}