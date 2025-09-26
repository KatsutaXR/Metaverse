using VContainer;

public class SimpleRoomObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.SimpleRoom;
    [Inject]
    public SimpleRoomObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _prefabDatabase = prefabDatabase;
        _networkController = networkController;
        _worldDatabase = worldDatabase;
    }
}
