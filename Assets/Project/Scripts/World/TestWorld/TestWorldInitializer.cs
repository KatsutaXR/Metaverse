using VContainer;
using UniRx;

/// <summary>
/// TestWorld参加時の初期化処理用クラス
/// </summary>
public class TestWorldInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.TestWorld;

    [Inject]
    public TestWorldInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase)
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
