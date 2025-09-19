using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class TestWorldNetworkController : WorldNetworkController
{
    private TestWorldObjectFactory _testWorldObjectFactory;
    [Inject]
    public TestWorldNetworkController(NetworkRunnerController runnerController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, TestWorldObjectFactory testWorldObjectFactory, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData)
    {
        _runner = runnerController.Runner;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _testWorldObjectFactory = testWorldObjectFactory;
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _playerData = playerData;
    }

    /// <summary>
    /// ワールド内の初期化処理を行う
    /// 同期オブジェクトに関しては基本各Spawned内で初期化させる
    /// </summary>
    public override void Initialize()
    {
        _runner.AddCallbacks(this);

        Debug.Log($"active = {SceneManager.GetActiveScene().name}");


        GameObject playerObject = _testWorldObjectFactory.CreatePlayer(_runner, _runner.LocalPlayer);
        _playerData.Player = playerObject;
        var playerReferences = playerObject.GetComponentInChildren<PlayerReferences>(true);

        // 鏡の設定
        GameObject[] mirrorObjects = GameObject.FindGameObjectsWithTag("Mirror");
        if (mirrorObjects.Length != 0)
        {
            foreach (var mirror in mirrorObjects)
            {
                mirror.GetComponent<MirrorView>().PlayerCamera = playerReferences.Camera;
            }
        }

        // todo:最初はSetActive = falseにする
        GameObject clientUI = _testWorldObjectFactory.CreateClientUI();
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), playerReferences);
        WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(worldUIView);

        _playerPresenter.Initialize(playerObject.GetComponentInChildren<PlayerView>(true));
    }

    public override void Dispose()
    {
        _runner.RemoveCallbacks(this);
    }

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 線の同期
        var strokeManagers = GameObject.FindObjectsByType<StrokeController>(FindObjectsSortMode.None);
        if (strokeManagers.Length != 0)
        {
            foreach (var manager in strokeManagers)
            {
                manager.SyncStrokes(player);
            }
        }
    }
}
