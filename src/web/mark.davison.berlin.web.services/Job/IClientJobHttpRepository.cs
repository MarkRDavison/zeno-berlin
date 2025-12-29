namespace mark.davison.berlin.web.services.Job;

public interface IClientJobHttpRepository
{
    Task<TResponse> PostLongPolling<TRequest, TResponse, TResponseData>(
        TRequest request,
        TimeSpan interval,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();

    Task<TResponse> PostSetupBackgroundJob<TRequest, TResponse, TResponseData>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();

    Task<TResponse> PostSetupBackgroundJob<TRequest, TResponse, TResponseData>(
        TRequest request,
        Func<TResponse, Task> callback,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new();
}
