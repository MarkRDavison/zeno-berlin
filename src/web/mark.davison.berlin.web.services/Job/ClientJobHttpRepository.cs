using System.Text.Json;

namespace mark.davison.berlin.web.services.Job;

public class ClientJobHttpRepository : IClientJobHttpRepository
{
    private IClientHttpRepository _clientHttpRepository;

    public ClientJobHttpRepository(IClientHttpRepository clientHttpRepository)
    {
        _clientHttpRepository = clientHttpRepository;
    }

    public async Task<TResponse> PostLongPolling<TRequest, TResponse, TResponseData>(
        TRequest request,
        TimeSpan interval,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new()
    {
        var response = await _clientHttpRepository.Post<TRequest, TResponse>(request, cancellationToken);

        Console.WriteLine("PostLongPolling Response: {0}", JsonSerializer.Serialize(response));

        if (!response.Success)
        {
            return response;
        }

        if (response.JobId == null)
        {
            response.Errors.Add("FAILED_CREATE_JOB");
            return response;
        }

        var initialDelay = TimeSpan.FromSeconds(10);

        await Task.Delay(initialDelay, cancellationToken);

        request.JobId = response.JobId;

        var maxTime = TimeSpan.FromSeconds(120);
        while (maxTime.TotalSeconds > 0 && !cancellationToken.IsCancellationRequested) // TODO: DUPLICATE
        {
            await Task.Delay(interval, cancellationToken);
            maxTime -= interval;

            response = await _clientHttpRepository.Post<TRequest, TResponse>(request, cancellationToken);

            if (response.JobStatus == "Complete" ||
                response.JobStatus == "Errored")
            {
                break;
            }
        }

        if (maxTime.TotalSeconds <= 0)
        {
            response.Errors.Add("FAILED_JOB_TIMEOUT");
        }

        return response;
    }

    public async Task<TResponse> PostSetupBackgroundJob<TRequest, TResponse, TResponseData>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new()
    {
        return await PostSetupBackgroundJob<TRequest, TResponse, TResponseData>(
            request,
            _ => Task.CompletedTask,
            cancellationToken);
    }

    public async Task<TResponse> PostSetupBackgroundJob<TRequest, TResponse, TResponseData>(
        TRequest request,
        Func<TResponse, Task> callback,
        CancellationToken cancellationToken)
        where TRequest : class, IJobRequest, ICommand<TRequest, TResponse>
        where TResponse : Response<TResponseData>, IJobResponse, new()
        where TResponseData : class, new()
    {
        var response = await _clientHttpRepository.Post<TRequest, TResponse>(request, cancellationToken);

        if (!response.Success || response.JobId == null)
        {
            return response;
        }

        _ = Task.Run(async () =>
        {
            TimeSpan interval = TimeSpan.FromSeconds(2);

            var asyncResponse = response;

            request.JobId = asyncResponse.JobId;

            var maxTime = TimeSpan.FromSeconds(120);
            while (maxTime.TotalSeconds > 0) // TODO: DUPLICATE
            {
                await Task.Delay(interval);
                maxTime -= interval;

                asyncResponse = await _clientHttpRepository.Post<TRequest, TResponse>(request, CancellationToken.None);

                if (asyncResponse.JobStatus == "Complete" ||
                    asyncResponse.JobStatus == "Errored")
                {
                    break;
                }
            }

            if (maxTime.TotalSeconds <= 0)
            {
                asyncResponse.Errors.Add("FAILED_JOB_TIMEOUT");
                return;
            }

            await callback(asyncResponse);
        });


        return response;
    }
}
