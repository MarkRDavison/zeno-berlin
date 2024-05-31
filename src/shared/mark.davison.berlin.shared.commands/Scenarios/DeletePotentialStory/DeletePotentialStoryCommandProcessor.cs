namespace mark.davison.berlin.shared.commands.Scenarios.DeletePotentialStory;

public sealed class DeletePotentialStoryCommandProcessor : ICommandProcessor<DeletePotentialStoryCommandRequest, DeletePotentialStoryCommandResponse>
{
    private readonly IDbContext _dbContext;

    public DeletePotentialStoryCommandProcessor(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeletePotentialStoryCommandResponse> ProcessAsync(DeletePotentialStoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var deleted = await _dbContext.DeleteEntityByIdAsync<PotentialStory>(request.Id, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (deleted == null)
        {
            ValidationMessages.CreateErrorResponse<DeletePotentialStoryCommandResponse>(
                ValidationMessages.FAILED_TO_FIND_ENTITY,
                nameof(PotentialStory),
                request.Id.ToString());
        }

        return new DeletePotentialStoryCommandResponse();
    }
}
