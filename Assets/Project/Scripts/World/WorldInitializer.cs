using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

/// <summary>
/// 各ワールドで行う共通の初期化処理をまとめるクラス
/// 各ワールドの初期化用クラスで継承して使う
/// </summary>
public abstract class WorldInitializer : IStartable
{
    protected WorldNetworkController _worldNetworkController;
    protected NetworkController _networkController;
    protected WorldDatabase _worldDatabase;
    public abstract WorldID TargetWorldID { get; }

    protected WorldInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _worldNetworkController = worldNetworkController;
        _networkController = networkController;
        _worldDatabase = worldDatabase;

        _networkController
            .LoadSceneCompleted
            .Take(1)
            .Subscribe(_ => Initialize());
    }

    protected virtual void InitializeBase()
    {
        // 初期化処理

        // ワールド用シーンをアクティブにする
        var worldData = _worldDatabase.GetWorldById(TargetWorldID);
        if (worldData == null)
        {
            Debug.LogError($"WorldData for {TargetWorldID} not found.");
            return;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(worldData.WorldName));

        _worldNetworkController.Initialize();
    }
    
    public virtual void Start() { }

    public abstract void Initialize();
}
