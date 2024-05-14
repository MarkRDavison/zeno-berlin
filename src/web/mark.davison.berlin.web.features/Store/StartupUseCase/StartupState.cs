namespace mark.davison.berlin.web.features.Store.StartupUseCase;

[FeatureState]
public sealed class StartupState
{
    public ReadOnlyCollection<UpdateTypeDto> UpdateTypes { get; }

    public StartupState() : this(Enumerable.Empty<UpdateTypeDto>())
    {

    }

    public StartupState(IEnumerable<UpdateTypeDto> updateTypes)
    {
        UpdateTypes = new([.. updateTypes]);
    }
}
