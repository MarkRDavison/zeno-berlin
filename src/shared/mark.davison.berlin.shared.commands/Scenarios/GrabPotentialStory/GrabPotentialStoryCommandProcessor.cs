namespace mark.davison.berlin.shared.commands.Scenarios.GrabPotentialStory;

public sealed class GrabPotentialStoryCommandProcessor : ICommandProcessor<GrabPotentialStoryCommandRequest, GrabPotentialStoryCommandResponse>
{
    private readonly IDbContext<BerlinDbContext> _dbContext;
    private readonly ICommandDispatcher _commandDispatcher;

    public GrabPotentialStoryCommandProcessor(
        IDbContext<BerlinDbContext> dbContext,
        ICommandDispatcher commandDispatcher)
    {
        _dbContext = dbContext;
        _commandDispatcher = commandDispatcher;
    }

    public async Task<GrabPotentialStoryCommandResponse> ProcessAsync(GrabPotentialStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

        var potentialStory = await _dbContext.GetByIdAsync<PotentialStory>(request.Id, cancellationToken);

        if (potentialStory == null)
        {
            return ValidationMessages.CreateErrorResponse<GrabPotentialStoryCommandResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(PotentialStory),
                request.Id.ToString());
        }

        var addStoryRequest = new AddStoryCommandRequest { StoryAddress = potentialStory.Address };

        var addStoryResponse = await _commandDispatcher.Dispatch<AddStoryCommandRequest, AddStoryCommandResponse>(addStoryRequest, cancellationToken);

        if (addStoryResponse.SuccessWithValue)
        {
            await _dbContext.DeleteEntityAsync(potentialStory, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitTransactionAsync(cancellationToken);

            return new()
            {
                Errors = [.. addStoryResponse.Errors],
                Warnings = [.. addStoryResponse.Warnings],
                Value = addStoryResponse.Value
            };
        }

        await transaction.RollbackTransactionAsync(cancellationToken);

        return new GrabPotentialStoryCommandResponse
        {
            Errors = [.. addStoryResponse.Errors],
            Warnings = [.. addStoryResponse.Warnings]
        };
    }
}
