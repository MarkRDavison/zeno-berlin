namespace mark.davison.berlin.api.jobs.Services;

public sealed class JobOrchestrationService : IJobOrchestrationService
{
    private readonly IDistributedPubSub _distributedPubSubService;
    private readonly ICheckJobsService _checkJobsService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDateService _dateService;
    private readonly JobsAppSettings _appSettings;
    private readonly ILogger<JobOrchestrationService> _logger;

    private bool _inProgress;
    private DateTime _inProgressSet;

    public JobOrchestrationService(
        IDistributedPubSub distributedPubSubService,
        ICheckJobsService checkJobsService,
        IServiceScopeFactory serviceScopeFactory,
        IDateService dateService,
        IOptions<JobsAppSettings> options,
        ILogger<JobOrchestrationService> logger)
    {
        _distributedPubSubService = distributedPubSubService;
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
        var inProgressTimeout = TimeSpan.FromMinutes(120);

        if (_inProgress)
        {
            if (_inProgressSet.Add(inProgressTimeout) < _dateService.Now)
            {
                return;
            }

            _inProgressSet = _dateService.Now;
            _inProgress = true;
        }

        var backoffIncrement = TimeSpan.FromSeconds(Random.Shared.Next(5, 10)); // TODO: Config???
        var timesBackedOff = 3;// TODO: Config???

        HashSet<Guid> performed = new();

        try
        {
            while (true)
            {
                var (lockAcquired, job) = await _checkJobsService.CheckForAvailableJob(performed, CancellationToken.None);

                if (job == null)
                {
                    if (!lockAcquired)
                    {
                        if (timesBackedOff > 0)
                        {
                            timesBackedOff--;
                            _logger.LogInformation("Failed to acquire a lock to check for jobs, waiting to check again");
                            await Task.Delay(backoffIncrement);
                            continue;
                        }

                        _logger.LogInformation("No jobs found to perform after trying multiple times");
                        return;
                    }

                    _logger.LogInformation("No jobs found to perform");
                    return;
                }

                _logger.LogInformation("Found job ({0}) to perform, status: {1}", job.Id, job.Status);

                var performedJob = await PerformJob(job, cancellationToken);

                performed.Add(job.Id);
                // TODO: Save something back to redis to indicate that this is done?? Save querying db constantly???

                // TODO: This seems to keep picking up this completed job next time around
            }
        }
        finally
        {
            _inProgressSet = DateTime.MinValue;
            _inProgress = false;
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

    private static async Task<object> PerformCommand<TRequest, TResponse>(TRequest request, ICommandHandler<TRequest, TResponse> handler, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
        where TRequest : class, ICommand<TRequest, TResponse>, new() where TResponse : Response, new()
    {
        return await handler.Handle(request, currentUserContext, cancellationToken);
    }

    private static async Task<Job> SaveJob(Job job, IDbContext<BerlinDbContext> dbContext, CancellationToken cancellationToken)
    {
        dbContext.Set<Job>().Update(job);

        await dbContext.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<Job> PerformJob(Job job, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext<BerlinDbContext>>();

        job.StartedAt = _dateService.Now;

        var requestType = Type.GetType(job.JobType);
        if (requestType == null)
        {
            job.Status = JobStatusConstants.Errored;
            job.JobResponse = JsonSerializer.Serialize(new Response
            {
                Errors = ["Failed to deserialize job, type not recongnised"]
            });

            return await SaveJob(job, dbContext, cancellationToken);
        }

        object? request = null;

        try
        {
            var options = JsonSerializerOptions.Default;
            options.MakeReadOnly();
            request = JsonSerializer.Deserialize(
                job.JobRequest,
                requestType,
                options);
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
            return await SaveJob(job, dbContext, cancellationToken);
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
            return await SaveJob(job, dbContext, cancellationToken);
        }

        var handlerType = ConstructCommandHandlerType(extractedRequestType, extractedResponseType);

        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();
        if (job.ContextUser is not null)
        {
            var identity = new ClaimsIdentity("Job");

            identity.AddClaim(new Claim(AuthConstants.InternalUserId, Guid.Empty.ToString()));
            identity.AddClaim(new Claim(AuthConstants.TenantId, TenantIds.SystemTenantId.ToString()));

            foreach (var userRole in job.ContextUser.UserRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, userRole.Role!.Name));
            }

            await currentUserContext.PopulateFromPrincipal(new ClaimsPrincipal(identity), "JOBS");
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

                var jobo = await SaveJob(job, dbContext, cancellationToken);

                Console.WriteLine("Finished job: {0} with status {1}", job.Id, job.Status);

                return jobo;
            }
        }

        job.Status = JobStatusConstants.Errored;
        job.LastModified = _dateService.Now;
        job.JobResponse = JsonSerializer.Serialize(new Response
        {
            Errors = ["Technical error submitting job to handler"]
        });

        return await SaveJob(job, dbContext, cancellationToken);
    }

    public async Task InitialiseJobMonitoring()
    {
        await _distributedPubSubService.SubscribeToKeyAsync(
            _appSettings.JOBS.JOB_CHECK_EVENT_KEY,
            OnJobCheckEvent);
    }
}
