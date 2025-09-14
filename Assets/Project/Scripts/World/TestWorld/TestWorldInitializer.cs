using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using UniRx;

/// <summary>
/// TestWorld参加時の初期化処理用クラス
/// </summary>
public class TestWorldInitializer : WorldInitializer
{
    private readonly TestWorldNetworkController _testWorldNetworkController;
    private readonly NetworkController _networkController;
    private readonly WorldDatabase _worldDatabase;
    [Inject]
    public TestWorldInitializer(TestWorldNetworkController testWorldNetworkController, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _testWorldNetworkController = testWorldNetworkController;
        _networkController = networkController;
        _worldDatabase = worldDatabase;

        _networkController
            .LoadSceneCompleted
            .Take(1)
            .Subscribe(_ => Initialize());
    }

    private void Initialize()
    {
        // 初期化処理

        // ワールド用シーンをアクティブにする
        var worldData = _worldDatabase.GetWorldById(WorldID.TestWorld);
        if (worldData == null)
        {
            Debug.LogError($"WorldData for {WorldID.TestWorld} not found.");
            return;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(worldData.WorldName));

        _testWorldNetworkController.Initialize();
    }

    public override void Start()
    {
        
    }
}
