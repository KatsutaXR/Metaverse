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

    protected virtual void InitializeBase()
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

        _worldNetworkController.Initialize();
    }
    public virtual void Start() { }

    public abstract void Initialize();
}
