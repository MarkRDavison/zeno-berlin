namespace mark.davison.berlin.web.features.Store;

public interface IStoreHelper // TODO: Move to common.
{
    IDisposable Force();
    IDisposable ConditionalForce(bool force);
    TimeSpan DefaultRefetchTimeSpan { get; }

    void Dispatch<TAction>(TAction action)
        where TAction : BaseAction;

    void DispatchWithThrottle<TAction>(DateTime lastDispatched, TAction action)
        where TAction : BaseAction;

    Task DispatchWithThrottleAndWaitForResponse<TAction, TActionResponse>(DateTime lastDispatched, TAction action)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new();

    Task<TActionResponse> DispatchAndWaitForResponse<TAction, TActionResponse>(TAction action)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new();

    Task<TActionResponse> DispatchAndWaitForResponse<TAction, TActionResponse>(TAction action, TimeSpan timeout)
        where TAction : BaseAction
        where TActionResponse : BaseActionResponse, new();
}
