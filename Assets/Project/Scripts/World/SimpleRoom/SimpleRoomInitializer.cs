using VContainer;
using UniRx;

/// <summary>
/// SimpleRoom参加時の初期化処理用クラス
/// </summary>
public class SimpleRoomInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.SimpleRoom;
    
    [Inject]
    public SimpleRoomInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _worldNetworkController = worldNetworkController;
        _networkController = networkController;
        _worldDatabase = worldDatabase;

        _networkController
            .LoadSceneCompleted
            .Take(1)
            .Subscribe(_ => Initialize());
    }

    public override void Initialize()
    {
        InitializeBase();
    }
}
