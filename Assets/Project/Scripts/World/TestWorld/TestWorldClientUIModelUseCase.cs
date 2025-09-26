using VContainer;

public class TestWorldClientUIModelUseCase : ClientUIModelUseCase
{
    public override WorldID TargetWorldID => WorldID.TestWorld;
    [Inject]
    public TestWorldClientUIModelUseCase(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }
}
