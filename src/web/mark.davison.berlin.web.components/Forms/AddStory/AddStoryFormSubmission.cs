using mark.davison.berlin.web.features.Store.StoryListUseCase;

namespace mark.davison.berlin.web.components.Forms.AddStory;

public class AddStoryFormSubmission : IFormSubmission<AddStoryFormViewModel>
{
    private readonly IDispatcher _dispatcher;
    private readonly IActionSubscriber _actionSubscriber;

    public AddStoryFormSubmission(
        IDispatcher dispatcher,
        IActionSubscriber actionSubscriber
    )
    {
        _dispatcher = dispatcher;
        _actionSubscriber = actionSubscriber;
    }

    public async Task<Response> Primary(AddStoryFormViewModel formViewModel)
    {
        var action = new AddStoryListAction
        {
            StoryAddress = formViewModel.StoryAddress
        };

        // TODO: Framework-itize this.
        TaskCompletionSource tcs = new();
        AddStoryListActionResponse? result = null;

        _actionSubscriber.SubscribeToAction(
            this,
            (AddStoryListActionResponse resultAction) =>
            {
                result = resultAction;
                tcs.SetResult();
            });

        using (_actionSubscriber.GetActionUnsubscriberAsIDisposable(this))
        {
            _dispatcher.Dispatch(action);

            await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(10)));
        }

        if (result == null)
        {
            return new Response { Errors = ["TODO: TIMED OUT, please try again..."] };
        }

        return result;
    }
}
