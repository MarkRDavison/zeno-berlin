namespace mark.davison.berlin.api.Data;

public interface IBerlinDataSeeder
{
    Task EnsureDataSeeded(CancellationToken cancellationToken);
}
