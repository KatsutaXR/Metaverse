using VContainer;

/// <summary>
/// SimpleRoom参加時の初期化処理用クラス
/// </summary>
public class SimpleRoomInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.SimpleRoom;
    
    [Inject]
    public SimpleRoomInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase) :
    base (worldNetworkController, networkController, worldDatabase) {}

    public override void Initialize()
    {
        InitializeBase();
    }
}
