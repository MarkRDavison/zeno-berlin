namespace mark.davison.berlin.web.features.Store.FandomListUseCase;

public class FandomListEffects
{
    private readonly IClientHttpRepository _repository;

    public FandomListEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchFandomListActionAsync(FetchFandomsListAction action, IDispatcher dispatcher)
    {
        var queryRequest = new FandomListQueryRequest();

        var queryResponse = await _repository.Get<FandomListQueryResponse, FandomListQueryRequest>(queryRequest, CancellationToken.None);

        var actionResponse = new FetchFandomsListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. queryResponse.Errors],
            Warnings = [.. queryResponse.Warnings],
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleEditFandomListActionAsync(EditFandomListAction action, IDispatcher dispatcher)
    {
        var commandRequest = new EditFandomCommandRequest
        {
            FandomId = action.FandomId,
            Changes =
            [
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(FandomDto.Name),
                    PropertyType = typeof(string).FullName!,
                    Value = action.Name
                },
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(FandomDto.IsHidden),
                    PropertyType = typeof(bool).FullName!,
                    Value = action.IsHidden
                },
                new DiscriminatedPropertyChangeset
                {
                    Name = nameof(FandomDto.ParentFandomId),
                    PropertyType = typeof(Guid?).FullName!,
                    Value = action.ParentFandomId
                }
            ]
        };

        var commandResponse = await _repository.Post<EditFandomCommandResponse, EditFandomCommandRequest>(commandRequest, CancellationToken.None);

        if (!commandResponse.SuccessWithValue)
        {
            // TODO: Re-query it????
            return;
        }

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new EditFandomListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleAddFandomListActionAsync(AddFandomListAction action, IDispatcher dispatcher)
    {
        var commandRequest = new AddFandomCommandRequest
        {
            Name = action.Name,
            ExternalName = action.Name,
            IsHidden = action.IsHidden,
            IsUserSpecified = true,
            ParentFandomId = action.ParentFandomId
        };

        var commandResponse = await _repository.Post<AddFandomCommandResponse, AddFandomCommandRequest>(commandRequest, CancellationToken.None);

        // TODO: Create response from response, copies Errors/Warnings in common
        var actionResponse = new AddFandomListActionResponse
        {
            ActionId = action.ActionId,
            Errors = [.. commandResponse.Errors],
            Warnings = [.. commandResponse.Warnings],
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

}
