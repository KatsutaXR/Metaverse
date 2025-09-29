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
    private RespawnAreaController _respawnAreaController;
    [Inject]
    public LobbyInitializer(LobbyObjectFactory lobbyObjectFactory, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData, GlobalNonNativeKeyboard keyboard, RespawnAreaController respawnAreaController)
    {
        _lobbyObjectFactory = lobbyObjectFactory;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _playerData = playerData;
        _keyboard = keyboard;
        _respawnAreaController = respawnAreaController;
    }

    public void Start()
    {
        GameObject player = _lobbyObjectFactory.CreatePlayer();
        _playerData.Player = player;
        Transform playerCamera = GameObject.FindWithTag("MainCamera").transform;

        var playerOrigin = GameObject.FindWithTag("Player").transform;
        _keyboard.playerRoot = playerOrigin;
        _keyboard.cameraTransform = playerCamera;

        _respawnAreaController.Initialize(playerOrigin, WorldID.None);
        
        GameObject[] mirrorObjects = GameObject.FindGameObjectsWithTag("Mirror");
        if (mirrorObjects.Length != 0)
        {
            foreach (var mirror in mirrorObjects)
            {
                mirror.GetComponent<MirrorView>().PlayerCamera = playerCamera;
            }
        }

        // todo:最初はSetActive = falseにする
        GameObject clientUI = _lobbyObjectFactory.CreateClientUI();
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), player.GetComponentInChildren<PlayerReferences>(true));

        WorldUIView worldUIView = GameObject.FindAnyObjectByType<WorldUIView>();
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(worldUIView);

        _playerPresenter.Initialize(player.GetComponentInChildren<PlayerView>(true));
    }

}
