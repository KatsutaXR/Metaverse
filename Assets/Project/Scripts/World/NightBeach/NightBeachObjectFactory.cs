using VContainer;

public class NightBeachObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.NightBeach;
    [Inject]
    public NightBeachObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _prefabDatabase = prefabDatabase;
        _networkController = networkController;
        _worldDatabase = worldDatabase;
    }
}
