namespace mark.davison.berlin.web.components.Pages.NewStory;

public partial class NewStoryViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IState<ManageStoryState> _manageStoryState;
    private readonly IState<FandomListState> _fandomState;
    private readonly IState<AuthorListState> _authorState;
    private readonly IState<StartupState> _startupState;
    private readonly IDispatcher _dispatcher;
    private readonly IClientNavigationManager _clientNavigationManager;
    private readonly IDialogService _dialogService;
    private readonly IActionSubscriber _actionSubscriber;
    private readonly IStoreHelper _storeHelper;
    private bool _disposedValue;
    public Guid Id { get; set; }

    public NewStoryViewModel(
        IState<ManageStoryState> manageStoryState,
        IState<FandomListState> fandomState,
        IState<AuthorListState> authorState,
        IState<StartupState> startupState,
        IDispatcher dispatcher,
        IClientNavigationManager clientNavigationManager,
        IDialogService dialogService,
        IActionSubscriber actionSubscriber,
        IStoreHelper storeHelper)
    {
        _manageStoryState = manageStoryState;
        _fandomState = fandomState;
        _authorState = authorState;
        _startupState = startupState;
        _dispatcher = dispatcher;
        _clientNavigationManager = clientNavigationManager;
        _dialogService = dialogService;
        _actionSubscriber = actionSubscriber;
        _storeHelper = storeHelper;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public StoryManageDto? Data => _manageStoryState.Value.Data;

    public void Initialise(Guid id)
    {
        Id = id;
        _manageStoryState.OnStateChange += OnStateChanged;
        _fandomState.OnStateChange += OnStateChanged;
        _authorState.OnStateChange += OnStateChanged;
        _startupState.OnStateChange += OnStateChanged;


        if (Id != default && !_manageStoryState.Value.IsLoading && Data.StoryId != Id)
        {
            // TODO: Brute force...
            _dispatcher.Dispatch(new FetchManageStoryAction { StoryId = Id });
            _dispatcher.Dispatch(new FetchAuthorsListAction());
            _dispatcher.Dispatch(new FetchFandomsListAction());
        }
    }

    public async Task OnConsumedChapterIconClicked(bool set)
    {
        if (set)
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
}
