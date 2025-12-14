namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

[Effect]
public sealed class AuthorListEffects
{
    private readonly IClientHttpRepository _repository;

    public AuthorListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleFetchAuthorListActionAsync(FetchAuthorsListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new AuthorListQueryRequest();

        var queryResponse = await _repository.Get<AuthorListQueryRequest, AuthorListQueryResponse>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchAuthorsListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        dispatcher.Dispatch(actionResponse);
    }
}
