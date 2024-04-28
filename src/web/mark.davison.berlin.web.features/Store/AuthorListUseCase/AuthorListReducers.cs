namespace mark.davison.berlin.web.features.Store.AuthorListUseCase;

public static class AuthorListReducers
{
    [ReducerMethod]
    public static AuthorListState FetchAuthorsListAction(AuthorListState state, FetchAuthorsListAction action)
    {
        return new AuthorListState(
            true,
            []);
    }

    [ReducerMethod]
    public static AuthorListState FetchAuthorsListActionResponse(AuthorListState state, FetchAuthorsListActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new AuthorListState(
                false,
                response.Value);
        }

        return new AuthorListState(
            false,
            []);
    }
}
