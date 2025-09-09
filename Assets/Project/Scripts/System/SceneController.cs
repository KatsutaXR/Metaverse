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

    [Inject]
    public SceneController(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }

    public async UniTask LoadLobbyAsync()
    {
        await SceneManager.LoadSceneAsync("LobbyScene", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LobbyScene"));
    }

    public async UniTask LoadWorldAsync(WorldID worldID)
    {
        var worldData = _worldDatabase.GetWorldById(worldID);
        if (worldData == null)
        {
            Debug.LogError($"WorldData for {worldID} not found.");
            return;
        }

        // Worldシーンのロード
        await SceneManager.LoadSceneAsync(worldData.WorldName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(worldData.WorldName));
    }

    public async UniTask UnloadWorldAsync(WorldID worldID)
    {
        await SceneManager.UnloadSceneAsync(_worldDatabase.GetWorldById(worldID).WorldName);
    }

    public async UniTask UnloadLobbyAsync()
    {
        await SceneManager.UnloadSceneAsync("LobbyScene");
    }
}
