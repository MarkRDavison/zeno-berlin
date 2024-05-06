namespace mark.davison.berlin.api.models.configuration.EntityConfiguration;

public class JobEntityConfiguration : BerlinEntityConfiguration<Job>
{
    public override void ConfigureEntity(EntityTypeBuilder<Job> builder)
    {
        builder.Property(_ => _.JobType);
        builder.Property(_ => _.JobRequest);
        builder.Property(_ => _.JobResponse);
        builder.Property(_ => _.Status);
        builder.Property(_ => _.SubmittedAt);
        builder.Property(_ => _.SelectedAt);
        builder.Property(_ => _.StartedAt);
        builder.Property(_ => _.FinishedAt);
        builder.Property(_ => _.PerformerId);
    }
}
