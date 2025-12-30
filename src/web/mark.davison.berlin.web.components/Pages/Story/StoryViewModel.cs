namespace mark.davison.berlin.web.components.Pages.Story;

public partial class StoryViewModel : IStoryViewModel
{
    private readonly IState<ManageStoryState> _manageStoryState;
    private readonly IState<FandomListState> _fandomState;
    private readonly IState<AuthorListState> _authorState;
    private readonly IState<StartupState> _startupState;
    private readonly IDispatcher _dispatcher;
    private readonly IClientNavigationManager _clientNavigationManager;
    private readonly IDialogService _dialogService;
    private readonly IStoreHelper _storeHelper;
    private bool _disposedValue;
    private bool _initialized;

    public Guid Id { get; set; }

    public StoryViewModel(
        IState<ManageStoryState> manageStoryState,
        IState<FandomListState> fandomState,
        IState<AuthorListState> authorState,
        IState<StartupState> startupState,
        IDispatcher dispatcher,
        IClientNavigationManager clientNavigationManager,
        IDialogService dialogService,
        IStoreHelper storeHelper)
    {
        _manageStoryState = manageStoryState;
        _fandomState = fandomState;
        _authorState = authorState;
        _startupState = startupState;
        _dispatcher = dispatcher;
        _clientNavigationManager = clientNavigationManager;
        _dialogService = dialogService;
        _storeHelper = storeHelper;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public StoryManageDto? Data => _manageStoryState.Value.Data;

    [MemberNotNullWhen(false, nameof(Data))]
    public bool ManageStoryLoading => Data is null || _manageStoryState.Value.IsLoading;
    public bool FandomsLoading => _fandomState.Value.IsLoading;
    public bool AuthorsLoading => _authorState.Value.IsLoading;
    public bool ConsumedChapterUpToDate => Data?.ConsumedChapters is not null && Data.ConsumedChapters == Data.CurrentChapters;
    public string CurrentChapterAddress => Data?.ConsumedChapters is not null
        ? Data.Updates.FirstOrDefault(_ => _.CurrentChapters == Data.ConsumedChapters + 1 && !string.IsNullOrEmpty(_.ChapterAddress))?.ChapterAddress ?? Data.Address
        : Data?.Address ?? string.Empty;
    public string LastCheckedText => $"Last checked {Data?.LastChecked.Humanize()}";
    public string LastAuthoredText => $"Last authored {Data?.LastAuthored.ToDateTime(TimeOnly.MinValue).Humanize()}";
    public string ChaptersText => $"Chapters: {Data?.CurrentChapters}/{Data?.TotalChapters?.ToString() ?? "?"}";
    public string UpdateTypeText
    {
        get
        {
            var updateType = _startupState.Value.Data.UpdateTypes.FirstOrDefault(_ => _.Id == Data?.UpdateTypeId);

            if (updateType is null)
            {
                return string.Empty;
            }

            return $"Updates {updateType.Description.ToLower()}";
        }
    }

    public IEnumerable<AuthorDto> DisplayAuthors => Data is null || AuthorsLoading
        ? []
        : Data.AuthorIds
            .Select(_ => LinkDisplayHelpers.GetAuthor(_, !ShowActualAuthors, _authorState.Value.Entities))
            .OfType<AuthorDto>()
            .DistinctBy(_ => _.AuthorId);

    public IEnumerable<FandomDto> DisplayFandoms => Data is null || FandomsLoading
        ? []
        : Data.FandomIds
            .Select(_ => LinkDisplayHelpers.GetFandom(_, !ShowActualFandoms, _fandomState.Value.Entities))
            .OfType<FandomDto>()
            .DistinctBy(_ => _.FandomId);

    // Observable properties?
    public bool InProgress { get; set; }

    private bool _showActualFandoms;
    public bool ShowActualFandoms
    {
        get => _showActualFandoms;
        set
        {
            if (_showActualFandoms == value)
            {
                return;
            }

            _showActualFandoms = value;

            NotifyChange(nameof(ShowActualFandoms));
            NotifyChange(nameof(DisplayFandoms));
        }
    }

    private bool _showActualAuthors;
    public bool ShowActualAuthors
    {
        get => _showActualAuthors;
        set
        {
            if (_showActualAuthors == value)
            {
                return;
            }

            _showActualAuthors = value;

            NotifyChange(nameof(ShowActualAuthors));
            NotifyChange(nameof(DisplayAuthors));
        }
    }

    public bool Initialise(Guid id)
    {
        if (_initialized)
        {
            return false;
        }

        _initialized = true;
        Id = id;
        _manageStoryState.OnStateChange += OnStateChanged;
        _fandomState.OnStateChange += OnStateChanged;
        _authorState.OnStateChange += OnStateChanged;
        _startupState.OnStateChange += OnStateChanged;

        if (Id != default && !_manageStoryState.Value.IsLoading && Data?.StoryId != Id)
        {
            // TODO: Brute force...
            _dispatcher.Dispatch(new FetchManageStoryAction { StoryId = Id });
            _dispatcher.Dispatch(new FetchAuthorsListAction());
            _dispatcher.Dispatch(new FetchFandomsListAction());
        }

        return true;
    }


    public async Task OnConsumedChapterIconClick(bool set, CancellationToken cancellationToken = default)
    {
        if (set && Data is not null)
        {
            // TODO: Change state data to records???
            // TODO: This SetStoryConsumedChaptersAction should have a reducer
            // that sets the current chapters in the store

            await _storeHelper.DispatchAndWaitForResponse<
                SetStoryConsumedChaptersAction,
                SetStoryConsumedChaptersActionResponse>(
                new SetStoryConsumedChaptersAction
                {
                    StoryId = Id,
                    ConsumedChapters = Data.CurrentChapters
                });
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        NotifyChange(null);
    }

    private void NotifyChange(string? name)
    {
        // TODO: Some form of throttling of changes?
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _manageStoryState.OnStateChange -= OnStateChanged;
                _fandomState.OnStateChange -= OnStateChanged;
                _authorState.OnStateChange -= OnStateChanged;
                _startupState.OnStateChange -= OnStateChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<Response> Delete(CancellationToken cancellationToken = default)
    {
        var action = new DeleteStoryAction { StoryId = Id };

        var response = await _storeHelper.DispatchAndWaitForResponse<DeleteStoryAction, DeleteStoryActionResponse>(action);

        if (response.Success)
        {
            // TODO: Dispatch action rather than navigate???
            _clientNavigationManager.NavigateTo(Routes.Dashboard);
        }

        return response;
    }

    public async Task CheckStory()
    {
        InProgress = true;

        var action = new UpdateStoryAction
        {
            StoryId = Id
        };

        if (Data?.Complete is true)
        {
            // Confirmation dialog???
            Console.Error.WriteLine("TODO: If the story is complete prompt with confirmation, user/server setting to configure this check???");
        }

        await _storeHelper.DispatchAndWaitForResponse<UpdateStoryAction, UpdateStoryActionResponse>(action);

        InProgress = false;
    }

    public async Task OpenDeleteConfirmationDialog()
    {
        var options = new DialogOptions // TODO: Standardize/settings service???
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = false
        };

        var param = new DialogParameters<ConfirmationDialog>
        {
            { _ => _.PrimaryText, "Delete" },
            { _ => _.Body, $"Are you sure you wish to delete {Data?.Name}?" },
            { _ => _.Color, Color.Error },
            { _ => _.PrimaryCallback, DeleteStory }
        };

        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>("Delete story", param, options);

        await dialog.Result;
    }

    public async Task OpenEditStoryDialog()
    {
        if (Data is not null)
        {
            var options = new DialogOptions // TODO: Standardize/settings service???
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,

                FullWidth = false
            };

            var param = new DialogParameters<
                FormModal<ModalViewModel<EditStoryFormViewModel, EditStoryForm>,
                EditStoryFormViewModel,
                EditStoryForm>>
            {
                { _ => _.PrimaryText, "Save" },
                { _ => _.Instance, new EditStoryFormViewModel
                    {
                        ConsumedChapters = Data.ConsumedChapters,
                        CurrentChapters = Data.CurrentChapters,
                        UpdateTypeId = Data.UpdateTypeId,
                        UpdateTypes = [.. _startupState.Value.Data.UpdateTypes],
                        StoryId = Id
                    }
                }
            };
        }
    }

    public async Task AddStoryUpdate()
    {
        if (Data is not null)
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var instance = new AddStoryUpdateFormViewModel
            {
                StoryId = Id,
                ExistingUpdates = [.. Data.Updates.Select(_ => new UpdateInfo(_.CurrentChapters, _.LastAuthored))]
            };

            var param = new DialogParameters<FormModal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>, AddStoryUpdateFormViewModel, AddStoryUpdateForm>>
            {
                { _ => _.PrimaryText, "Save" },
                { _ => _.Instance, instance }
            };

            var dialog = await _dialogService.ShowAsync<
                FormModal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>,
                AddStoryUpdateFormViewModel,
                AddStoryUpdateForm>>("Add Story Update", param, options);

            await dialog.Result;
        }
    }

    private async Task<Response> DeleteStory()
    {
        var action = new DeleteStoryAction { StoryId = Id };

        var response = await _storeHelper.DispatchAndWaitForResponse<DeleteStoryAction, DeleteStoryActionResponse>(action);

        if (response.Success)
        {
            _clientNavigationManager.NavigateTo(Routes.Dashboard);
        }

        return response;
    }

    public void FavouriteClick(bool set)
    {
        _dispatcher.Dispatch(new SetStoryFavouriteAction
        {
            StoryId = Id,
            IsFavourite = set
        });
    }

    public string UpdateChapterText(StoryManageUpdatesDto update)
    {
        return $"{update.CurrentChapters}/{update.TotalChapters?.ToString() ?? "?"}";
    }
}
