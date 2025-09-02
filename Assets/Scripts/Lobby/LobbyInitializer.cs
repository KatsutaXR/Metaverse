using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Lobby参加時の初期化処理用クラス
/// </summary>
public class LobbyInitializer : IStartable
{
    private LobbyObjectFactory _lobbyObjectFactory;
    private PrefabDatabase _prefabDatabase;
    private WorldDatabase _worldDatabase;
    private ClientUIPresenter _clientUIPresenter;
    private WorldUIPresenter _worldUIPresenter;
    private PlayerPresenter _playerPresenter;
    private PlayerData _playerData;
    [Inject]
    public LobbyInitializer(LobbyObjectFactory lobbyObjectFactory, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData)
    {
        _lobbyObjectFactory = lobbyObjectFactory;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _playerData = playerData;
    }

    public void Start()
    {
        GameObject player = _lobbyObjectFactory.CreatePlayer();
        _playerData.Player = player;

        // todo:最初はSetActive = falseにする
        GameObject clientUI = _lobbyObjectFactory.CreateClientUI();
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), player.GetComponentInChildren<AvatarData>(true));
        WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(clientUI.GetComponentInChildren<WorldUIView>(true));

        _playerPresenter.Initialize(player.GetComponentInChildren<PlayerView>(true));
    }

}
