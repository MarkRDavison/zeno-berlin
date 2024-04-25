﻿using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.EditFandom;

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
}
