namespace mark.davison.berlin.web.components.Forms.AddStoryUpdate;

public sealed record UpdateInfo(int Chapter, DateOnly AuthoredDate);

public sealed class AddStoryUpdateFormViewModel : IFormViewModel
{
    public Guid StoryId { get; set; }
    public int? CurrentChapters { get; set; }
    public int? TotalChapters { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool Complete { get; set; }
    public DateTime MaxDate => DetermineMaxDate();
    public DateTime MinDate => DetermineMinDate();

    private int? _infoChapter;
    private UpdateInfo? _before;
    private UpdateInfo? _same;
    private UpdateInfo? _after;

    private List<UpdateInfo> _existingUpdates = [];
    public List<UpdateInfo> ExistingUpdates
    {
        get => _existingUpdates;
        set
        {
            _existingUpdates = value.OrderBy(_ => _.Chapter).ToList();
        }
    }

    public bool ValidateAgainstExistingUpdates()
    {
        SyncRelatedInfo(CurrentChapters);
        var result =
            UpdateDate != null &&
            (_before == null || _before.AuthoredDate <= DateOnly.FromDateTime(UpdateDate!.Value)) &&
            _same == null &&
            (_after == null || _after.AuthoredDate >= DateOnly.FromDateTime(UpdateDate!.Value));

        return result;
    }

    private void SyncRelatedInfo(int? currentChapter)
    {
        if (_infoChapter == currentChapter) { return; }
        _infoChapter = currentChapter;
        if (currentChapter == null)
        {
            _before = null;
            _same = null;
            _after = null;
            return;
        }

        _before = ExistingUpdates.LastOrDefault(_ => _.Chapter < currentChapter);
        _same = ExistingUpdates.FirstOrDefault(_ => _.Chapter == currentChapter);
        _after = ExistingUpdates.FirstOrDefault(_ => _.Chapter > currentChapter);
    }

    private DateTime DetermineMaxDate()
    {
        SyncRelatedInfo(CurrentChapters);

        if (_after == null)
        {
            return DateTime.Today;
        }

        return new DateTime(_after.AuthoredDate, TimeOnly.MaxValue);
    }

    private DateTime DetermineMinDate()
    {
        SyncRelatedInfo(CurrentChapters);

        if (_before == null)
        {
            return DateTime.MinValue;
        }

        return new DateTime(_before.AuthoredDate, TimeOnly.MinValue);
    }

    public string? ValidateCurrentChapters()
    {
        SyncRelatedInfo(CurrentChapters);

        if (CurrentChapters != null && _same != null)
        {
            return "There is already an update for this chapter"; // TODO: Better validation framework/localisation
        }

        return null;
    }

    public bool Valid =>
        CurrentChapters != null &&
        CurrentChapters.Value > 0 &&
        UpdateDate != null &&
        UpdateDate != DateTime.MinValue &&
        UpdateDate != DateTime.MaxValue &&
        ValidateAgainstExistingUpdates();
}
