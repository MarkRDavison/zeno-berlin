namespace mark.davison.berlin.api.Process;

public interface IProcess
{

}

public interface IProcess<T> : IProcess
{
    Task<Response<T>> ProcessAsync(CancellationToken cancellationToken);
}
