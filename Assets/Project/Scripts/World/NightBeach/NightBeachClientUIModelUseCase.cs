using VContainer;

public class NightBeachClientUIModelUseCase : ClientUIModelUseCase
{
    public override WorldID TargetWorldID => WorldID.NightBeach;
    [Inject]
    public NightBeachClientUIModelUseCase(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }
}
