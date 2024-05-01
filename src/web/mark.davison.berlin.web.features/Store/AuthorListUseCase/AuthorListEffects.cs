namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public class AuthorListEffects
{
    private readonly IClientHttpRepository _repository;

    public AuthorListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchAuthorListActionAsync(FetchAuthorsListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new AuthorListQueryRequest();

        var queryResponse = await _repository.Get<AuthorListQueryResponse, AuthorListQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchAuthorsListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
