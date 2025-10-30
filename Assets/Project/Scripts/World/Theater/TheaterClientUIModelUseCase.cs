using VContainer;

public class TheaterClientUIModelUseCase : ClientUIModelUseCase
{
    public override WorldID TargetWorldID => WorldID.Theater;
    [Inject]
    public TheaterClientUIModelUseCase(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }
}
