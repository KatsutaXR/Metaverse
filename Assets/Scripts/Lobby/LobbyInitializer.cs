using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
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
    private GlobalNonNativeKeyboard _keyboard;
    [Inject]
    public LobbyInitializer(LobbyObjectFactory lobbyObjectFactory, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData, GlobalNonNativeKeyboard keyboard)
    {
        _lobbyObjectFactory = lobbyObjectFactory;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _playerData = playerData;
        _keyboard = keyboard;
    }

    public void Start()
    {
        GameObject player = _lobbyObjectFactory.CreatePlayer();
        _playerData.Player = player;
        _keyboard.playerRoot = player.transform.Find("XR Origin (XR Rig)");
        _keyboard.cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();

        // todo:最初はSetActive = falseにする
        GameObject clientUI = _lobbyObjectFactory.CreateClientUI();
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), player.GetComponentInChildren<AvatarData>(true));
        WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(clientUI.GetComponentInChildren<WorldUIView>(true));

        _playerPresenter.Initialize(player.GetComponentInChildren<PlayerView>(true));
    }

}
