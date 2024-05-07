using mark.davison.berlin.shared.models.dtos;
using System.Text.Json;

namespace mark.davison.berlin.shared.commands.Scenarios.Export;

public class ExportCommandProcessor : ICommandProcessor<ExportCommandRequest, ExportCommandResponse>
{
    private readonly IRepository _repository;

    public ExportCommandProcessor(
        IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExportCommandResponse> ProcessAsync(ExportCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new ExportCommandResponse();

        // TODO: Move this to ValidateAndProcessCommandHandler???
        if (request is IJobRequest &&
            response is IJobResponse &&
            request.UseJob)
        {
            await using (_repository.BeginTransaction())
            {
                if (request.JobId != null)
                {
                    var existingJob = await _repository.GetEntityAsync<Job>(request.JobId.Value, cancellationToken);

                    if (existingJob == null)
                    {
                        return ValidationMessages.CreateErrorResponse<ExportCommandResponse>(
                            ValidationMessages.FAILED_TO_FIND_ENTITY,
                            nameof(Job),
                            request.JobId.Value.ToString());
                    }

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

                    response.Value = JsonSerializer.Deserialize<SerialisedtDataDto>(existingJob.JobResponse);

                    return response;
                }

                var job = await _repository.UpsertEntityAsync(new Job
                {
                    Id = Guid.NewGuid(),
                    ContextUserId = currentUserContext.CurrentUser.Id,
                    JobType = typeof(ExportCommandRequest).AssemblyQualifiedName!,
                    JobRequest = JsonSerializer.Serialize(request),
                    Status = JobStatusConstants.Submitted,
                    SubmittedAt = DateTime.UtcNow,
                    UserId = currentUserContext.CurrentUser.Id,
                    LastModified = DateTime.UtcNow,
                    Created = DateTime.UtcNow
                }, cancellationToken);

                // TODO: Trigger job redis pub/sub

                if (job == null)
                {
                    return ValidationMessages.CreateErrorResponse<ExportCommandResponse>(
                        ValidationMessages.ERROR_SAVING);
                }

                response.JobId = job.Id;
            }

            return response;
        }


        await using (_repository.BeginTransaction())
        {
            var stories = await _repository
                .QueryEntities<Story>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .ToListAsync();

            var storyUpdates = await _repository
                .QueryEntities<StoryUpdate>()
                .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
                .ToListAsync();

            var exportData = new SerialisedtDataDto
            {
                Version = 1,
                Stories = [.. stories.Select(s => CreateSerialisedStoryDto(s, storyUpdates.Where(u => u.StoryId == s.Id)))]
            };

            response.Value = exportData;
        }

        return response;
    }

    private SerialisedStoryDto CreateSerialisedStoryDto(Story story, IEnumerable<StoryUpdate> storyUpdates)
    {
        return new SerialisedStoryDto
        {
            StoryAddress = story.Address,
            Favourite = story.Favourite,
            Updates = [.. storyUpdates.OrderByDescending(_ => _.LastAuthored).Select(CreateSerialisedStoryUpdateDto)]
        };
    }

    private SerialisedStoryUpdateDto CreateSerialisedStoryUpdateDto(StoryUpdate update)
    {
        return new SerialisedStoryUpdateDto
        {
            Complete = update.Complete,
            CurrentChapters = update.CurrentChapters,
            TotalChapters = update.TotalChapters,
            LastAuthored = update.LastAuthored
        };
    }
}
