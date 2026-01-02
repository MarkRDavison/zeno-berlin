namespace mark.davison.berlin.web.components.Pages.Story;

public interface IStoryViewModel : INotifyPropertyChanged, IDisposable
{
    [MemberNotNullWhen(false, nameof(Data))]
    bool ManageStoryLoading { get; }
    bool FandomsLoading { get; }
    bool AuthorsLoading { get; }
    StoryManageDto? Data { get; }
    bool InProgress { get; }
    bool IsCheckStoryDisabled { get; }
    bool ConsumedChapterUpToDate { get; }
    bool ShowActualFandoms { get; set; }
    bool ShowActualAuthors { get; set; }
    string CurrentChapterAddress { get; }
    string LastCheckedText { get; }
    string LastAuthoredText { get; }
    string ChaptersText { get; }
    string UpdateTypeText { get; }
    IEnumerable<AuthorDto> DisplayAuthors { get; }
    IEnumerable<FandomDto> DisplayFandoms { get; }

    bool Initialise(Guid id);
    void FavouriteClick(bool set);
    string UpdateChapterText(StoryManageUpdatesDto update);

    Task<Response> Delete(CancellationToken cancellationToken = default);
    Task OnConsumedChapterIconClick(bool set, CancellationToken cancellationToken = default);
    Task OpenEditStoryDialog();
    Task CheckStory();
    Task OpenDeleteConfirmationDialog();
    Task AddStoryUpdate();
}