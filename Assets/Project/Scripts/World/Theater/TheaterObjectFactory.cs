using VContainer;

public class TheaterObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.Theater;
    [Inject]
    public TheaterObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase) :
    base(prefabDatabase, networkController, worldDatabase) {}
}
