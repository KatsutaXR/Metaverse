using VContainer;
using UniRx;

/// <summary>
/// NightBeach参加時の初期化処理用クラス
/// </summary>
public class NightBeachInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.NightBeach;
    
    [Inject]
    public NightBeachInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase)
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
