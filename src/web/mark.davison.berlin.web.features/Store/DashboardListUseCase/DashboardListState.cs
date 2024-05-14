namespace mark.davison.berlin.web.features.Store.DashboardListUseCase;

[FeatureState]
public sealed class DashboardListState
{
    public bool IsLoading { get; }
    public ReadOnlyCollection<DashboardTileDto> Entities { get; }
    public DateTime LastLoaded { get; }

    public DashboardListState() : this(false, Enumerable.Empty<DashboardTileDto>(), DateTime.MinValue)
    {

    }

    public DashboardListState(bool isLoading, IEnumerable<DashboardTileDto> stories, DateTime lastLoaded)
    {
        IsLoading = isLoading;
        Entities = new ReadOnlyCollection<DashboardTileDto>(stories.ToList());
        LastLoaded = lastLoaded;
    }
}
