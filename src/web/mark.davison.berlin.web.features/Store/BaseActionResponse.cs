namespace mark.davison.berlin.web.features.Store;

public class BaseActionResponse : Response
{
    public Guid ActionId { get; set; }
}

public class BaseActionResponse<T> : BaseActionResponse
{
    [MemberNotNullWhen(returnValue: true, nameof(Response<T>.Value))]
    public bool SuccessWithValue => Success && Value != null;
    public T? Value { get; set; }
}
