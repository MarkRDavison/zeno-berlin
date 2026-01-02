namespace mark.davison.berlin.web.components.tests.Pages.Story;

public sealed class StoryViewModelTests
{
    private readonly Mock<IState<ManageStoryState>> _manageStoryState;
    private readonly Mock<IState<FandomListState>> _fandomListState;
    private readonly Mock<IState<AuthorListState>> _authorListState;
    private readonly Mock<IState<StartupState>> _startupState;
    private readonly Mock<IDispatcher> _dispatcher;
    private readonly Mock<IClientNavigationManager> _clientNavigationManager;
    private readonly Mock<IDialogService> _dialogService;
    private readonly Mock<IStoreHelper> _storyHelper;

    private readonly StoryViewModel _viewModel;

    public StoryViewModelTests()
    {
        _manageStoryState = new();
        _fandomListState = new();
        _authorListState = new();
        _startupState = new();
        _dispatcher = new();
        _clientNavigationManager = new();
        _dialogService = new();
        _storyHelper = new();

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(() => new ManageStoryState());
        _fandomListState
            .Setup(_ => _.Value)
            .Returns(() => new FandomListState());
        _authorListState
            .Setup(_ => _.Value)
            .Returns(() => new AuthorListState());
        _startupState
            .Setup(_ => _.Value)
            .Returns(() => new StartupState());

        _viewModel = new StoryViewModel(
            _manageStoryState.Object,
            _fandomListState.Object,
            _authorListState.Object,
            _startupState.Object,
            _dispatcher.Object,
            _clientNavigationManager.Object,
            _dialogService.Object,
            _storyHelper.Object);
    }

    private static StoryManageDto CreateDummyStoryManageDto()
    {
        return new StoryManageDto(
            Guid.NewGuid(),
            string.Empty,
            string.Empty,
            0,
            null,
            null,
            false,
            false,
            false,
            UpdateTypeConstants.EachChapterId,
            DateTime.UtcNow,
            DateOnly.FromDateTime(DateTime.Today),
            [],
            [],
            []);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task ManageStoryLoading_ReturnsExpected(bool loaded)
    {
        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(
                !loaded,
                loaded
                    ? CreateDummyStoryManageDto()
                    : null));

        await Assert.That(_viewModel.ManageStoryLoading).IsNotEqualTo(loaded);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FandomsLoading_ReturnsExpected(bool loaded)
    {
        _fandomListState
            .Setup(_ => _.Value)
            .Returns(new FandomListState(!loaded, []));

        await Assert.That(_viewModel.FandomsLoading).IsNotEqualTo(loaded);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AuthorsLoading_ReturnsExpected(bool loaded)
    {
        _authorListState
            .Setup(_ => _.Value)
            .Returns(new AuthorListState(!loaded, []));

        await Assert.That(_viewModel.AuthorsLoading).IsNotEqualTo(loaded);
    }

    [Test]
    public async Task Initialise_FirstCall_DispatchesFetchActions()
    {
        var storyId = Guid.NewGuid();

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, null));

        _viewModel.Initialise(storyId);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchManageStoryAction>()),
            Times.Once);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchAuthorsListAction>()),
            Times.Once);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchFandomsListAction>()),
            Times.Once);
    }

    [Test]
    public async Task Initialise_FirstCall_Subscribes()
    {
        var storyId = Guid.NewGuid();
        var notifyCount = 0;

        _viewModel.PropertyChanged += (_, _) => notifyCount++;

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, CreateDummyStoryManageDto()));

        _viewModel.Initialise(storyId);

        _manageStoryState.Raise(_ => _.OnStateChange += null!, EventArgs.Empty);

        await Assert.That(notifyCount).IsEqualTo(1);
    }

    [Test]
    public async Task Initialise_SecondCall_DoesNothing()
    {
        var storyId = Guid.NewGuid();

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, null));

        var firstResult = _viewModel.Initialise(storyId);
        var secondResult = _viewModel.Initialise(Guid.NewGuid());

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchManageStoryAction>()),
            Times.Once);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchAuthorsListAction>()),
            Times.Once);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchFandomsListAction>()),
            Times.Once);

        await Assert.That(secondResult).IsFalse();
    }

    [Test]
    public async Task Initialise_SecondCall_DoesNotDuplicateStateSubscriptions()
    {
        var storyId = Guid.NewGuid();
        var notifyCount = 0;

        _viewModel.PropertyChanged += (_, _) => notifyCount++;

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, CreateDummyStoryManageDto()));

        _viewModel.Initialise(storyId);
        _viewModel.Initialise(storyId);

        _manageStoryState.Raise(_ => _.OnStateChange += null!, EventArgs.Empty);

        await Assert.That(notifyCount).IsEqualTo(1);
    }

    [Test]
    public async Task Initialise_WithDefaultId_DoesNotDispatchFetchActions()
    {
        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, null));

        _viewModel.Initialise(Guid.Empty);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchManageStoryAction>()),
            Times.Never);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchAuthorsListAction>()),
            Times.Never);

        _dispatcher.Verify(
            _ => _.Dispatch(It.IsAny<FetchFandomsListAction>()),
            Times.Never);
    }

    [Test]
    public async Task Dispose_UnsubscribesFromAllStateChangeEvents()
    {
        _viewModel.Initialise(Guid.NewGuid());

        _viewModel.Dispose();

        _manageStoryState.VerifyRemove(_ => _.OnStateChange -= It.IsAny<EventHandler>(), Times.Once);
        _fandomListState.VerifyRemove(_ => _.OnStateChange -= It.IsAny<EventHandler>(), Times.Once);
        _authorListState.VerifyRemove(_ => _.OnStateChange -= It.IsAny<EventHandler>(), Times.Once);
        _startupState.VerifyRemove(_ => _.OnStateChange -= It.IsAny<EventHandler>(), Times.Once);
    }

    [Test]
    public async Task LastCheckedText_WhenDataNull_DoesNotThrow()
    {
        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, null));

        await Assert.That(() => _viewModel.LastCheckedText).ThrowsNothing();
    }

    [Test]
    public async Task LastAuthoredText_WhenDataPresent_FormatsCorrectly()
    {
        var authoredDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(
                false,
                CreateDummyStoryManageDto() with { LastAuthored = authoredDate }
            ));

        await Assert.That(_viewModel.LastAuthoredText)
            .StartsWith("Last authored ");
    }

    [Test]
    public async Task ChaptersText_WithTotalChaptersNull_UsesQuestionMark()
    {
        var dto = CreateDummyStoryManageDto() with
        {
            CurrentChapters = 5,
            TotalChapters = null
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await Assert.That(_viewModel.ChaptersText)
            .IsEqualTo("Chapters: 5/?");
    }

    [Test]
    public async Task UpdateTypeText_WhenUpdateTypeMissing_ReturnsEmptyString()
    {
        var dto = CreateDummyStoryManageDto();

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _startupState
            .Setup(_ => _.Value)
            .Returns(new StartupState(false, true, new StartupDataDto()));

        await Assert.That(_viewModel.UpdateTypeText)
            .IsEqualTo(string.Empty);
    }

    [Test]
    public async Task UpdateTypeText_WhenUpdateTypePresent_ReturnsFormattedString()
    {
        var updateType = new UpdateTypeDto
        {
            Id = UpdateTypeConstants.EachChapterId,
            Description = "Each chapter"
        };

        var dto = CreateDummyStoryManageDto() with
        {
            UpdateTypeId = updateType.Id
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _startupState
            .Setup(_ => _.Value)
            .Returns(new StartupState(false, true, new StartupDataDto
            {
                UpdateTypes = [updateType]
            }));

        await Assert.That(_viewModel.UpdateTypeText)
            .IsEqualTo("Updates each chapter");
    }

    [Test]
    public async Task ConsumedChapterUpToDate_WhenConsumedEqualsCurrent_IsTrue()
    {
        var dto = CreateDummyStoryManageDto() with
        {
            CurrentChapters = 10,
            ConsumedChapters = 10
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await Assert.That(_viewModel.ConsumedChapterUpToDate)
            .IsTrue();
    }

    [Test]
    public async Task ConsumedChapterUpToDate_WhenConsumedDiffers_IsFalse()
    {
        var dto = CreateDummyStoryManageDto() with
        {
            CurrentChapters = 10,
            ConsumedChapters = 5
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await Assert.That(_viewModel.ConsumedChapterUpToDate)
            .IsFalse();
    }

    [Test]
    public async Task CurrentChapterAddress_WhenMatchingUpdateExists_UsesUpdateAddress()
    {
        var updateAddress = "https://example.com/chapter/6";

        var updates = new[]
        {
            new StoryManageUpdatesDto
            {
                ChapterAddress = updateAddress,
                ChapterTitle = "Chapter 6",
                Complete = false,
                CurrentChapters = 6,
                TotalChapters = 10,
                LastAuthored = DateOnly.FromDateTime(DateTime.UtcNow),
                LastChecked = DateTime.UtcNow
            }
        };

        var dto = CreateDummyStoryManageDto() with
        {
            Address = "https://example.com/story",
            CurrentChapters = 6,
            ConsumedChapters = 5,
            Updates = [.. updates]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await Assert.That(_viewModel.CurrentChapterAddress)
            .IsEqualTo(updateAddress);
    }

    [Test]
    public async Task CurrentChapterAddress_WhenNoMatchingUpdate_FallsBackToStoryAddress()
    {
        var storyAddress = "https://example.com/story";

        var updates = new[]
        {

            new StoryManageUpdatesDto
            {
                ChapterAddress = "https://example.com/chapter/3",
                ChapterTitle = "Chapter 3",
                Complete = false,
                CurrentChapters = 3,
                TotalChapters = 10,
                LastAuthored = DateOnly.FromDateTime(DateTime.UtcNow),
                LastChecked = DateTime.UtcNow
            }
        };

        var dto = CreateDummyStoryManageDto() with
        {
            Address = storyAddress,
            CurrentChapters = 6,
            ConsumedChapters = 5,
            Updates = [.. updates]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await Assert.That(_viewModel.CurrentChapterAddress)
            .IsEqualTo(storyAddress);
    }

    [Test]
    public async Task DisplayAuthors_WhenShowActualAuthorsFalse_UsesMaskedAuthors()
    {
        var parentAuthorId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            AuthorIds = [authorId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _authorListState
            .Setup(_ => _.Value)
            .Returns(new AuthorListState(false,
            [
                new AuthorDto
                {
                    AuthorId = parentAuthorId,
                    Name = "Parent Author"
                },
                new AuthorDto
                {
                    AuthorId = authorId,
                    ParentAuthorId = parentAuthorId,
                    Name = "Actual Author"
                }
            ]));

        _viewModel.ShowActualAuthors = false;

        var authors = _viewModel.DisplayAuthors.ToList();

        await Assert.That(authors.Single().Name)
            .IsNotEqualTo("Actual Author");
    }

    [Test]
    public async Task DisplayAuthors_WhenShowActualAuthorsTrue_UsesActualAuthors()
    {
        var authorId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            AuthorIds = [authorId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _authorListState
            .Setup(_ => _.Value)
            .Returns(new AuthorListState(false,
            [
                new AuthorDto
                {
                    AuthorId = authorId,
                    Name = "Actual Author"
                }
            ]));

        _viewModel.ShowActualAuthors = true;

        var authors = _viewModel.DisplayAuthors.ToList();

        await Assert.That(authors.Single().Name)
            .IsEqualTo("Actual Author");
    }

    [Test]
    public async Task DisplayAuthors_DeduplicatesAuthors()
    {
        var authorId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            AuthorIds = [authorId, authorId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _authorListState
            .Setup(_ => _.Value)
            .Returns(new AuthorListState(false,
            [
                new AuthorDto
                {
                    AuthorId = authorId,
                    Name = "Author"
                }
            ]));

        var authors = _viewModel.DisplayAuthors.ToList();

        await Assert.That(authors.Count)
            .IsEqualTo(1);
    }

    [Test]
    public async Task DisplayFandoms_WhenLoading_ReturnsEmpty()
    {
        var dto = CreateDummyStoryManageDto() with
        {
            FandomIds = [Guid.NewGuid()]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _fandomListState
            .Setup(_ => _.Value)
            .Returns(new FandomListState(true, []));

        await Assert.That(_viewModel.DisplayFandoms)
            .IsEmpty();
    }

    [Test]
    public async Task DisplayFandoms_WhenShowActualFandomsFalse_UsesMaskedFandoms()
    {
        var parentFandomId = Guid.NewGuid();
        var fandomId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            FandomIds = [fandomId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _fandomListState
            .Setup(_ => _.Value)
            .Returns(new FandomListState(false,
            [
                new FandomDto
                {
                    FandomId = parentFandomId,
                    Name = "Parent Fandom"
                },
                new FandomDto
                {
                    FandomId = fandomId,
                    ParentFandomId = parentFandomId,
                    Name = "Actual Fandom"
                }
            ]));

        _viewModel.ShowActualFandoms = false;

        var fandoms = _viewModel.DisplayFandoms.ToList();

        await Assert.That(fandoms.Single().Name)
            .IsNotEqualTo("Actual Fandom");
    }

    [Test]
    public async Task DisplayFandoms_WhenShowActualFandomsTrue_UsesActualFandoms()
    {
        var fandomId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            FandomIds = [fandomId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _fandomListState
            .Setup(_ => _.Value)
            .Returns(new FandomListState(false,
            [
                new FandomDto
                {
                    FandomId = fandomId,
                    Name = "Actual Fandom"
                }
            ]));

        _viewModel.ShowActualFandoms = true;

        var fandoms = _viewModel.DisplayFandoms.ToList();

        await Assert.That(fandoms.Single().Name)
            .IsEqualTo("Actual Fandom");
    }

    [Test]
    public async Task DisplayFandoms_DeduplicatesFandoms()
    {
        var fandomId = Guid.NewGuid();

        var dto = CreateDummyStoryManageDto() with
        {
            FandomIds = [fandomId, fandomId]
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _fandomListState
            .Setup(_ => _.Value)
            .Returns(new FandomListState(false,
            [
                new FandomDto
                {
                    FandomId = fandomId,
                    Name = "Fandom"
                }
            ]));

        var fandoms = _viewModel.DisplayFandoms.ToList();

        await Assert.That(fandoms.Count)
            .IsEqualTo(1);
    }

    [Test]
    public async Task ShowActualAuthors_WhenChanged_RaisesPropertyChanged()
    {
        var raised = new HashSet<string?>();
        _viewModel.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        _viewModel.ShowActualAuthors = true;

        await Assert.That(raised).Contains(nameof(_viewModel.ShowActualAuthors));
    }

    [Test]
    public async Task ShowActualAuthors_WhenChanged_RaisesDisplayAuthorsChanged()
    {
        var raised = new HashSet<string?>();
        _viewModel.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        _viewModel.ShowActualAuthors = true;

        await Assert.That(raised).Contains(nameof(_viewModel.DisplayAuthors));
    }

    [Test]
    public async Task ShowActualFandoms_WhenChanged_RaisesPropertyChanged()
    {
        var raised = new HashSet<string?>();
        _viewModel.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        _viewModel.ShowActualFandoms = true;

        await Assert.That(raised).Contains(nameof(_viewModel.ShowActualFandoms));
    }

    [Test]
    public async Task ShowActualFandoms_WhenChanged_RaisesDisplayFandomsChanged()
    {
        var raised = new HashSet<string?>();
        _viewModel.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        _viewModel.ShowActualFandoms = true;

        await Assert.That(raised).Contains(nameof(_viewModel.DisplayFandoms));
    }

    [Test]
    public async Task InProgress_WhenSet_DoesNotSilentlyFail()
    {
        _viewModel.InProgress = true;

        await Assert.That(_viewModel.InProgress).IsTrue();
    }

    [Test]
    public async Task OnStateChanged_RaisesPropertyChangedWithNullName()
    {
        string? name = "not-null";
        _viewModel.PropertyChanged += (_, e) => name = e.PropertyName;

        _viewModel.Initialise(Guid.NewGuid());

        _manageStoryState.Raise(_ => _.OnStateChange += null!, EventArgs.Empty);

        await Assert.That(name).IsNull();
    }

    [Test]
    public async Task StateChange_FromAnySubscribedState_TriggersNotification()
    {
        var count = 0;
        _viewModel.PropertyChanged += (_, _) => count++;

        _viewModel.Initialise(Guid.NewGuid());

        _authorListState.Raise(_ => _.OnStateChange += null!, EventArgs.Empty);

        await Assert.That(count).IsGreaterThan(0);
    }

    [Test]
    public async Task FavouriteClick_DispatchesSetStoryFavouriteAction()
    {
        _viewModel.FavouriteClick(true);

        _dispatcher.Verify(
            _ => _.Dispatch(It.Is<SetStoryFavouriteAction>(a => a.IsFavourite)),
            Times.Once);
    }

    [Test]
    public async Task OnConsumedChapterIconClick_WhenSetFalse_DoesNothing()
    {
        await _viewModel.OnConsumedChapterIconClick(false);

        _storyHelper.Verify(
            _ => _.DispatchAndWaitForResponse<
                SetStoryConsumedChaptersAction,
                SetStoryConsumedChaptersActionResponse>(It.IsAny<SetStoryConsumedChaptersAction>()),
            Times.Never);
    }

    [Test]
    public async Task OnConsumedChapterIconClick_WhenSetTrue_DispatchesConsumedChaptersAction()
    {
        var dto = CreateDummyStoryManageDto() with
        {
            CurrentChapters = 3,
            ConsumedChapters = 2
        };

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        await _viewModel.OnConsumedChapterIconClick(true);

        _storyHelper.Verify(
            _ => _.DispatchAndWaitForResponse<
                SetStoryConsumedChaptersAction,
                SetStoryConsumedChaptersActionResponse>(
                It.IsAny<SetStoryConsumedChaptersAction>()),
            Times.Once);
    }

    [Test]
    public async Task CheckStory_SetsInProgressTrueDuringExecution()
    {
        var tcs = new TaskCompletionSource<Response>();

        _storyHelper
            .Setup(_ => _.DispatchAndWaitForResponse<UpdateStoryAction, UpdateStoryActionResponse>(
                It.IsAny<UpdateStoryAction>()))
            .Returns(async () =>
            {
                await tcs.Task;
                return new UpdateStoryActionResponse
                {
                    Value = new()
                };
            });

        var task = _viewModel.CheckStory();

        await Assert.That(_viewModel.InProgress).IsTrue();

        tcs.SetResult(new Response());

        await task;
    }

    [Test]
    public async Task CheckStory_SetsInProgressFalseAfterCompletion()
    {
        _storyHelper
            .Setup(_ => _.DispatchAndWaitForResponse<UpdateStoryAction, UpdateStoryActionResponse>(
                It.IsAny<UpdateStoryAction>()))
            .ReturnsAsync(new UpdateStoryActionResponse
            {
                Value = new()
            });

        await _viewModel.CheckStory();

        await Assert.That(_viewModel.InProgress).IsFalse();
    }

    [Test]
    public async Task Delete_WhenSuccessful_NavigatesToDashboard()
    {
        _storyHelper
            .Setup(_ => _.DispatchAndWaitForResponse<DeleteStoryAction, DeleteStoryActionResponse>(
                It.IsAny<DeleteStoryAction>()))
            .ReturnsAsync(new DeleteStoryActionResponse
            {
                Errors = []
            });

        await _viewModel.Delete();

        _clientNavigationManager.Verify(
            _ => _.NavigateTo(Routes.Dashboard),
            Times.Once);
    }

    [Test]
    public async Task Delete_WhenUnsuccessful_DoesNotNavigate()
    {
        _storyHelper
            .Setup(_ => _.DispatchAndWaitForResponse<DeleteStoryAction, DeleteStoryActionResponse>(
                It.IsAny<DeleteStoryAction>()))
            .ReturnsAsync(new DeleteStoryActionResponse
            {
                Errors = ["Some error"]
            });

        await _viewModel.Delete();

        _clientNavigationManager.Verify(
            _ => _.NavigateTo(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public async Task OpenDeleteConfirmationDialog_ShowsDialogWithCorrectParameters()
    {
        _dialogService
            .Setup(_ => _.ShowAsync<ConfirmationDialog>(
                It.IsAny<string>(),
                It.IsAny<DialogParameters<ConfirmationDialog>>(),
                It.IsAny<DialogOptions>()))
            .ReturnsAsync(Mock.Of<IDialogReference>());

        await _viewModel.OpenDeleteConfirmationDialog();

        _dialogService.Verify(
            _ => _.ShowAsync<ConfirmationDialog>(
                "Delete story",
                It.IsAny<DialogParameters<ConfirmationDialog>>(),
                It.IsAny<DialogOptions>()),
            Times.Once);
    }

    [Test]
    public async Task AddStoryUpdate_ShowsDialog_WhenDataPresent()
    {
        var dto = CreateDummyStoryManageDto();

        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, dto));

        _dialogService
            .Setup(_ => _.ShowAsync<
                FormModal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>,
                AddStoryUpdateFormViewModel,
                AddStoryUpdateForm>>(
                It.IsAny<string>(),
                It.IsAny<DialogParameters<FormModal<ModalViewModel<AddStoryUpdateFormViewModel, AddStoryUpdateForm>,
                    AddStoryUpdateFormViewModel,
                    AddStoryUpdateForm>>>(),
                It.IsAny<DialogOptions>()))
            .ReturnsAsync(Mock.Of<IDialogReference>());

        await _viewModel.AddStoryUpdate();

        _dialogService.VerifyAll();
    }

    [Test]
    public async Task OpenEditStoryDialog_WhenDataNull_DoesNothing()
    {
        _manageStoryState
            .Setup(_ => _.Value)
            .Returns(new ManageStoryState(false, null));

        await _viewModel.OpenEditStoryDialog();

        _dialogService.VerifyNoOtherCalls();
    }

}
