using VContainer;

public class SimpleRoomObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.SimpleRoom;
    [Inject]
    public SimpleRoomObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase) :
    base(prefabDatabase, networkController, worldDatabase) {}
}
