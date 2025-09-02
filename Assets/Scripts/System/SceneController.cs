using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

/// <summary>
/// プロジェクト全体のシーンを制御するクラス
/// </summary>
public class SceneController
{
    private readonly WorldDatabase _worldDatabase;
    private readonly NetworkController _networkController;

    private WorldID _currentWorldID;
    private bool _isLobbyScene;

    [Inject]
    public SceneController(WorldDatabase worldDatabase, NetworkController networkController)
    {
        _worldDatabase = worldDatabase;
        _networkController = networkController;

        _isLobbyScene = false;
        _currentWorldID = WorldID.None;
    }

    public async UniTask LoadLobbyAsync()
    {
        // 現在Lobbyシーンにいるなら早期リターン
        if (_isLobbyScene) return;

        // 現在のWorldシーンがあればアンロード&ワールドから退出
        if (_currentWorldID != WorldID.None)
        {
            await _networkController.LeaveWorldAsync();

            await SceneManager.UnloadSceneAsync(_worldDatabase.GetWorldById(_currentWorldID).WorldName);
            _currentWorldID = WorldID.None;
        }

        await SceneManager.LoadSceneAsync("LobbyScene", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LobbyScene"));

        // ロビーに参加
        await _networkController.JoinLobbyAsync();

        _isLobbyScene = true;
    }

    public async UniTask LoadWorldAsync(WorldID worldID)
    {
        var worldData = _worldDatabase.GetWorldById(worldID);
        if (worldData == null)
        {
            Debug.LogError($"WorldData for {worldID} not found.");
            return;
        }

        // todo:ネットワークの状態や対象ワールドの人数上限に達しているかなど参加可否の条件を確認する

        // 現在のWorldシーンがあればアンロード
        if (_currentWorldID != WorldID.None)
        {
            await SceneManager.UnloadSceneAsync(_worldDatabase.GetWorldById(_currentWorldID).WorldName);
        }

        // Worldシーンのロード
        await SceneManager.LoadSceneAsync(worldData.WorldName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(worldData.WorldName));
        _currentWorldID = worldData.WorldID;
        _isLobbyScene = false;

        // ネットワークへの参加
        await _networkController.JoinWorldAsync();
    }
}
