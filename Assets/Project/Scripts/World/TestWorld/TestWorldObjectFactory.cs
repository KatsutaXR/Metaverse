using VContainer;

public class TestWorldObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.TestWorld;
    [Inject]
    public TestWorldObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _prefabDatabase = prefabDatabase;
        _networkController = networkController;
        _worldDatabase = worldDatabase;
    }
}
