namespace mark.davison.berlin.web.features.Store;

public class BaseActionResponse<T> : Response<T>
{
    public Guid ActionId { get; set; }
}
