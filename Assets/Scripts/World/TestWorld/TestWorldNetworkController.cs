using System.Linq;
using Fusion;
using UnityEngine;
using VContainer;

public class TestWorldNetworkController : WorldNetworkController
{
    private TestWorldObjectFactory _testWorldObjectFactory;
    [Inject]
    public TestWorldNetworkController(NetworkRunner runner, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, TestWorldObjectFactory testWorldObjectFactory, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData)
    {
        _runner = runner;
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

        if (player == runner.LocalPlayer)
        {
            GameObject playerObject = _testWorldObjectFactory.CreatePlayer(runner, player);
            _playerData.Player = playerObject;

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
