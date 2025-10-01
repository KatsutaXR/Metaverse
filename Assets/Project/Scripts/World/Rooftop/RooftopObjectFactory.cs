using VContainer;

public class RooftopObjectFactory : WorldObjectFactory
{
    public override WorldID TargetWorldID => WorldID.Rooftop;
    [Inject]
    public RooftopObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase) :
    base(prefabDatabase, networkController, worldDatabase) {}
}
