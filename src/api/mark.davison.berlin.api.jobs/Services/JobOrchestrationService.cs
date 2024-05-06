using mark.davison.berlin.shared.constants;
using mark.davison.common;
using mark.davison.common.CQRS;
using mark.davison.common.server.abstractions.CQRS;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace mark.davison.berlin.api.jobs.Services;

public class JobOrchestrationService : IJobOrchestrationService
{
    private readonly IRedisService _redisService;
    private readonly ICheckJobsService _checkJobsService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateService _dateService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<JobOrchestrationService> _logger;

    public JobOrchestrationService(
        IRedisService redisService,
        ICheckJobsService checkJobsService,
        IServiceScopeFactory serviceScopeFactory,
        IDateService dateService,
        IOptions<AppSettings> options,
        ILogger<JobOrchestrationService> logger)
    {
        _redisService = redisService;
        _checkJobsService = checkJobsService;
        _serviceScopeFactory = serviceScopeFactory;
        _dateService = dateService;
        _appSettings = options.Value;
        _logger = logger;
    }

    private async Task OnJobCheckEvent()
    {
        await RunAnyAvailableJobs(CancellationToken.None);
    }

    public async Task RunAnyAvailableJobs(CancellationToken cancellationToken)
    {
        var backoffIncrement = TimeSpan.FromSeconds(Random.Shared.Next(5, 10)); // TODO: Config???
        var timesBackedOff = 3;// TODO: Config???
        while (true)
        {
            var (lockAcquired, job) = await _checkJobsService.CheckForAvailableJob(CancellationToken.None);

            if (job == null)
            {
                if (!lockAcquired)
                {
                    if (timesBackedOff > 0)
                    {
                        timesBackedOff--;
                        await Task.Delay(backoffIncrement);
                        _logger.LogInformation("Failed to acquire a lock to check for jobs, waiting to check again");
                        continue;
                    }

                    _logger.LogInformation("No jobs found to perform after trying multiple times");
                    return;
                }

                _logger.LogInformation("No jobs found to perform");
                return;
            }

            _logger.LogInformation("Found job ({0}) to perform", job.Id);

            var performedJob = await PerformJob(job, cancellationToken);

            // TODO: Save something back to redis to indicate that this is done?? Save querying db constantly???
        }
    }

    public static Type ConstructCommandHandlerType(Type requestType, Type responseType)
    {
        var handlerType = typeof(ICommandHandler<,>);

        return handlerType.MakeGenericType(requestType, responseType);
    }

    // TODO: Helper??? Used in multiple places or should just be source gen which wont work here
    public static (Type? requestType, Type? responseType) ExtractCommandRequestResponseTypes(Type type)
    {

        var interfaces = type.GetInterfaces();
        var commandType = typeof(ICommand<,>);

        Type? responseType = null;

        var commandImplementation = interfaces.FirstOrDefault(_ =>
        {
            if (_.IsGenericType && _.IsConstructedGenericType)
            {
                var generic = _.GetGenericTypeDefinition();

                return generic == commandType;
            }

            return false;
        });

        if (commandImplementation != null)
        {
            var genericArguments = commandImplementation.GetGenericArguments();
            if (genericArguments.Length == 2)
            {
                if (genericArguments[0] == type)
                {
                    responseType = genericArguments[1];
                }
            }
        }

        return (type, responseType);
    }

    private static async Task<object> PerformCommand<TResponse, TRequest>(TRequest request, ICommandHandler<TRequest, TResponse> handler, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
        where TRequest : class, ICommand<TRequest, TResponse>, new() where TResponse : Response, new()
    {
        return await handler.Handle(request, currentUserContext, cancellationToken);
    }

    private static async Task<Job> SaveJob(Job job, IRepository repository, CancellationToken cancellationToken)
    {
        return await repository.UpsertEntityAsync(job, cancellationToken) ?? job;
    }

    public async Task<Job> PerformJob(Job job, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

        await using (repository.BeginTransaction())
        {
            job.StartedAt = _dateService.Now;

            var requestType = Type.GetType(job.JobType);
            if (requestType == null)
            {
                job.Status = JobStatusConstants.Errored;
                job.JobResponse = JsonSerializer.Serialize(new Response
                {
                    Errors = ["Failed to deserialize job, type not recongnised"]
                });

                return await SaveJob(job, repository, cancellationToken);
            }

            object? request = null;

            try
            {
                request = JsonSerializer.Deserialize(
                    job.JobRequest,
                    JsonTypeInfo.CreateJsonTypeInfo(
                        requestType,
                        SerializationHelpers.CreateStandardSerializationOptions()));
            }
            catch (Exception e)
            {
                job.Status = JobStatusConstants.Errored;
                job.LastModified = _dateService.Now;
                job.JobResponse = JsonSerializer.Serialize(new Response
                {
                    Errors = ["Failed to deserialize job", e.Message]
                });
            }

            if (request == null)
            {
                return await SaveJob(job, repository, cancellationToken);
            }

            var (extractedRequestType, extractedResponseType) = ExtractCommandRequestResponseTypes(request.GetType());

            if (extractedRequestType == null || extractedResponseType == null)
            {
                job.Status = JobStatusConstants.Errored;
                job.LastModified = _dateService.Now;
                job.JobResponse = JsonSerializer.Serialize(new Response
                {
                    Errors = ["Unknown job type", request.GetType().FullName ?? request.GetType().Name]
                });
                return await SaveJob(job, repository, cancellationToken);
            }

            var handlerType = ConstructCommandHandlerType(extractedRequestType, extractedResponseType);

            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();
            if (job.ContextUser != null)
            {
                currentUserContext.CurrentUser = job.ContextUser;
            }

            var methodInfo = typeof(JobOrchestrationService).GetMethod(nameof(PerformCommand), BindingFlags.Static | BindingFlags.NonPublic)!;

            var method = methodInfo.MakeGenericMethod(extractedRequestType, extractedResponseType);

            var responseTask = method.Invoke(null, [request, handler, currentUserContext, CancellationToken.None]);

            if (responseTask is Task<object> t)
            {
                var responseObject = await t;

                if (responseObject is Response responseValue)
                {
                    job.Status = JobStatusConstants.Complete;
                    job.LastModified = _dateService.Now;
                    job.JobResponse = JsonSerializer.Serialize(responseObject);

                    return await SaveJob(job, repository, cancellationToken);
                }
            }

            job.Status = JobStatusConstants.Errored;
            job.LastModified = _dateService.Now;
            job.JobResponse = JsonSerializer.Serialize(new Response
            {
                Errors = ["Technical error submitting job to handler"]
            });

            return await SaveJob(job, repository, cancellationToken);
        }
    }

    public async Task InitialiseJobMonitoring()
    {
        await _redisService.SubscribeToKeyAsync(
            _appSettings.JOB_CHECK_EVENT_KEY_NAME,
            "set",
            OnJobCheckEvent);
    }
}
