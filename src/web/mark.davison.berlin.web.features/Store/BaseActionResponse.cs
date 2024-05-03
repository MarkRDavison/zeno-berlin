namespace mark.davison.berlin.web.features.Store;

public class BaseActionResponse : Response // TODO: Move to common.
{
    public Guid ActionId { get; set; }
}

public class BaseActionResponse<T> : BaseActionResponse // TODO: Move to common.
{
    public BaseActionResponse()
    {

    }
    public BaseActionResponse(BaseAction action)
    {
        ActionId = action.ActionId;
    }

    [MemberNotNullWhen(returnValue: true, nameof(Response<T>.Value))]
    public bool SuccessWithValue => Success && Value != null;
    public T? Value { get; set; }
}
