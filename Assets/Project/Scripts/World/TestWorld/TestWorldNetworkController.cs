using System.Linq;
using Fusion;
using UnityEngine;
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

    public override void Initialize()
    {
        _runner.AddCallbacks(this);
    }

    // 各クライアント参加時の処理を行う
    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Joined TestWorld");

        // シーン上のネットワークオブジェクトをマスタークライアントが最初にスポーンする
        if (runner.IsSharedModeMasterClient)
        {
            var sceneObjects = GameObject.FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
            foreach (var obj in sceneObjects)
            {
                runner.Spawn(obj, obj.transform.position, obj.transform.rotation);
            }
        }

        if (player == runner.LocalPlayer)
            {
                GameObject playerObject = _testWorldObjectFactory.CreatePlayer(runner, player);
                _playerData.Player = playerObject;
                GameObject[] mirrorObjects = GameObject.FindGameObjectsWithTag("Mirror");
                if (mirrorObjects.Length != 0)
                {
                    Transform playerCamera = GameObject.FindWithTag("MainCamera").transform;
                    foreach (var mirror in mirrorObjects)
                    {
                        mirror.GetComponent<MirrorView>().PlayerCamera = playerCamera;
                    }
                }

                // todo:最初はSetActive = falseにする
                GameObject clientUI = _testWorldObjectFactory.CreateClientUI();
                _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), playerObject.GetComponentInChildren<AvatarData>(true));
                WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
                worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
                _worldUIPresenter.Initialize(clientUI.GetComponentInChildren<WorldUIView>(true));

                _playerPresenter.Initialize(playerObject.GetComponentInChildren<PlayerView>(true));
            }

    }

    public override void Dispose()
    {
        _runner.RemoveCallbacks(this);
    }
}
