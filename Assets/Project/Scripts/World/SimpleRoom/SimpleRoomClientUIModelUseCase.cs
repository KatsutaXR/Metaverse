using VContainer;

public class SimpleRoomClientUIModelUseCase : ClientUIModelUseCase
{
    public override WorldID TargetWorldID => WorldID.SimpleRoom;
    [Inject]
    public SimpleRoomClientUIModelUseCase(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }
}
