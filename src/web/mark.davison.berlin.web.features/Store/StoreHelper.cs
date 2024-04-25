namespace mark.davison.berlin.web.features.Store;

public class StoreHelper : IStoreHelper
{
    internal bool _force;

    private readonly IDateService _dateService;
    private readonly IDispatcher _dispatcher;
    private readonly IActionSubscriber _actionSubscriber;

    private class StoreHelperDisposable : IDisposable
    {
        private bool _disposedValue;
        private readonly StoreHelper _stateHelper;

        public StoreHelperDisposable(StoreHelper stateHelper)
        {
            _stateHelper = stateHelper;
            _stateHelper._force = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stateHelper._force = false;
                    _disposedValue = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

    public StoreHelper(
        IDateService dateService,
        IDispatcher dispatcher,
        IActionSubscriber actionSubscriber)
    {
        _dateService = dateService;
        _dispatcher = dispatcher;
        _actionSubscriber = actionSubscriber;
    }

    public IDisposable Force() => new StoreHelperDisposable(this);

    private bool RequiresRefetch(DateTime stateLastModified, TimeSpan interval)
    {
        return _force || _dateService.Now - stateLastModified > interval;
    }

    public TimeSpan DefaultRefetchTimeSpan => TimeSpan.FromMinutes(1);


    public void Dispatch<TAction>(TAction action)
        where TAction : BaseAction
    {
        _dispatcher.Dispatch(action);
    }

    public void DispatchWithThrottle<TAction>(DateTime lastDispatched, TAction action)
        where TAction : BaseAction
    {
        if (!RequiresRefetch(lastDispatched, DefaultRefetchTimeSpan))
        {
            return;
        }

        _dispatcher.Dispatch(action);
    }

    public async Task DispatchWithThrottleAndWaitForResponse<TAction, TActionResponse>(DateTime lastDispatched, TAction action)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new()
    {
        if (!RequiresRefetch(lastDispatched, DefaultRefetchTimeSpan))
        {
            return;
        }

        await DispatchAndWaitForResponse<TAction, TActionResponse>(action);
    }


    public async Task<TActionResponse> DispatchAndWaitForResponse<TAction, TActionResponse>(TAction action)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new()
    {
        return await DispatchAndWaitForResponse<TAction, TActionResponse>(action, TimeSpan.FromSeconds(100));
    }

    public async Task<TActionResponse> DispatchAndWaitForResponse<TAction, TActionResponse>(TAction action, TimeSpan timeout)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new()
    {
        TaskCompletionSource tcs = new();
        TActionResponse? result = null;

        _actionSubscriber.SubscribeToAction(
                this,
                (TActionResponse actionResponse) =>
                {
                    if (actionResponse.ActionId == action.ActionId)
                    {
                        result = actionResponse;
                        tcs.SetResult();
                    }
                });

        using (_actionSubscriber.GetActionUnsubscriberAsIDisposable(this))
        {
            _dispatcher.Dispatch(action);

            await Task.WhenAny(tcs.Task, Task.Delay(timeout));
        }

        if (result == null)
        {
            return new TActionResponse
            {
                ActionId = action.ActionId,
                Errors = ["TODO: TIMED OUT, please try again..."]
            };
        }

        return result;
    }
}
