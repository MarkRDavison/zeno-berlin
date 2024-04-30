namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public static class AuthorListReducers
{
    [ReducerMethod]
    public static AuthorListState FetchAuthorsListAction(AuthorListState state, FetchAuthorsListAction action)
    {
        return new AuthorListState(
            false,
            []);
    }

    [ReducerMethod]
    public static AuthorListState FetchAuthorsListActionResponse(AuthorListState state, FetchAuthorsListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            var returnedAuthorIds = response.Value.Select(_ => _.AuthorId).ToHashSet();

            return new AuthorListState(
                false,
                [.. state.Entities.Where(_ => !returnedAuthorIds.Contains(_.AuthorId)), .. response.Value]);
        }

        return new AuthorListState(
            false,
            []);
    }
}
