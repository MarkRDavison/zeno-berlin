namespace mark.davison.berlin.web.services.CommonCandidates;

public interface IClientJobHttpRepository
{
    Task<TResponse> PostLongPolling<TResponse, TRequest, TResponseData>(
        TRequest request,
        TimeSpan interval,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();

    Task<TResponse> PostSetupBackgroundJob<TResponse, TRequest, TResponseData>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();

    Task<TResponse> PostSetupBackgroundJob<TResponse, TRequest, TResponseData>(
        TRequest request,
        Func<TResponse, Task> callback,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();
}
