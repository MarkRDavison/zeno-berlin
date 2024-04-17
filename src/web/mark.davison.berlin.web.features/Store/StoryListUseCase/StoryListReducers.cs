namespace mark.davison.berlin.web.features.Store.StoryListUseCase;

public static class StoryListReducers
{
    [ReducerMethod]
    public static StoryListState AddStoryAction(StoryListState state, AddStoryListAction action)
    {
        return new StoryListState(
            state.IsLoading,
            [
                .. state.Stories.Where(_ => _.Id != action.ActionId),
                new StoryDto
                {
                    Id = action.ActionId,
                    Address = action.StoryAddress
                }
            ]);
    }

    [ReducerMethod]
    public static StoryListState AddStoryActionResponse(StoryListState state, AddStoryListActionResponse action)
    {
        // TODO: [MemberNotNullWhen( returnValue: true , nameof(BaseResponse<T>.Value))] in common
        if (action.SuccessWithValue)
        {
            return new StoryListState(
                state.IsLoading,
                [
                    .. state.Stories.Where(_ => _.Id != action.ActionId),
                    action.Value!
                ]);
        }

        return new StoryListState(
            state.IsLoading,
            [
                .. state.Stories.Where(_ => _.Id != action.ActionId)
            ]);
    }
}
