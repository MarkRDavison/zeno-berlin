namespace mark.davison.shared.server.services.CommonCandiates;

// TODO: Think this over
/*
 * This basically is for commands that you will invoke from the ui
 * So you can Trigger it, and get a job id back, and poll using that id
 * This is not for server cron jobs, this is only for manual ones
 * For server cron jobs you should not submit through here, just create the 'Job' record.
 * You can trigger one of these from a cron, just dont set UseJob, because then the job will create a job
 */
public abstract class ValidateAndProcessJobCommandHandler<TRequest, TResponse, TResponseData> : ICommandHandler<TRequest, TResponse>
    where TRequest : class, ICommand<TRequest, TResponse>, IJobRequest, new()
    where TResponse : Response<TResponseData>, IJobResponse, new()
    where TResponseData : class, new()
{
    private readonly ICommandProcessor<TRequest, TResponse> _processor;
    private readonly ICommandValidator<TRequest, TResponse>? _validator;
    private readonly IRepository _repository;
    private readonly IDistributedPubSub _distributedPubSub;

    public ValidateAndProcessJobCommandHandler(
        ICommandProcessor<TRequest, TResponse> processor,
        IRepository repository,
        IDistributedPubSub distributedPubSub)
    {
        _processor = processor;
        _validator = null;
        _repository = repository;
        _distributedPubSub = distributedPubSub;
    }

    public ValidateAndProcessJobCommandHandler(
        ICommandProcessor<TRequest, TResponse> processor,
        ICommandValidator<TRequest, TResponse> validator,
        IRepository repository,
        IDistributedPubSub distributedPubSub)
    {
        _processor = processor;
        _validator = validator;
        _repository = repository;
        _distributedPubSub = distributedPubSub;
    }

    protected abstract string NotificationKey { get; }

    public async Task<TResponse> Handle(TRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        if (command.UseJob)
        {
            await using (_repository.BeginTransaction())
            {
                if (command.JobId != null)
                {
                    return await HandleExistingJobId(command.JobId.Value, command, cancellation);
                }

                return await HandleCreateJob(command, currentUserContext, cancellation);
            }
        }
        else
        {
            if (_validator != null)
            {
                TResponse val = await _validator.ValidateAsync(command, currentUserContext, cancellation);
                if (!val.Success)
                {
                    return val;
                }
            }

            return await _processor.ProcessAsync(command, currentUserContext, cancellation);
        }
    }

    private async Task<TResponse> HandleCreateJob(TRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        command.UseJob = false; // When this is picked up and run. It should just be regular
        var job = await _repository.UpsertEntityAsync(new Job
        {
            Id = Guid.NewGuid(),
            ContextUserId = currentUserContext.CurrentUser.Id,
            JobType = typeof(ExportCommandRequest).AssemblyQualifiedName!,
            JobRequest = JsonSerializer.Serialize(command),
            Status = JobStatusConstants.Submitted,
            SubmittedAt = DateTime.UtcNow,
            UserId = currentUserContext.CurrentUser.Id,
            LastModified = DateTime.UtcNow,
            Created = DateTime.UtcNow
        }, cancellation);

        if (job == null)
        {
            return new TResponse()
            {
                Errors = ["FAILED_TO_CREATE_JOB"]
            };
        };

        if (command.TriggerImmediateJob)
        {
            await _distributedPubSub.TriggerNotificationAsync(NotificationKey, cancellation);
        }

        return new TResponse
        {
            JobId = job.Id
        };
    }

    private async Task<TResponse> HandleExistingJobId(Guid jobId, TRequest command, CancellationToken cancellationToken)
    {
        var existingJob = await _repository.GetEntityAsync<Job>(jobId, cancellationToken);

        if (existingJob == null)
        {
            return ValidationMessages.CreateErrorResponse<TResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(Job),
                jobId.ToString());
        }

        var response = new TResponse();

        var message = existingJob.Status switch
        {
            JobStatusConstants.Submitted => "Waiting to be picked up",
            JobStatusConstants.Running => "Still running",
            JobStatusConstants.Selected => "Still running",
            JobStatusConstants.Errored => "Job failed",
            _ => string.Empty
        };

        response.JobStatus = existingJob.Status;

        if (!string.IsNullOrEmpty(message))
        {
            response.Warnings.Add(message);
            return response;
        }

        response.Value = JsonSerializer.Deserialize<TResponseData>(existingJob.JobResponse);

        return response;
    }
}