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
    private ClientUIModel _clientUIModel;
    private WorldUIPresenter _worldUIPresenter;
    private ProfileUIPresenter _profileUIPresenter;
    private PlayerPresenter _playerPresenter;
    private GlobalNonNativeKeyboard _keyboard;
    private RespawnAreaController _respawnAreaController;
    [Inject]
    public LobbyInitializer(LobbyObjectFactory lobbyObjectFactory, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter, GlobalNonNativeKeyboard keyboard, RespawnAreaController respawnAreaController)
    {
        _lobbyObjectFactory = lobbyObjectFactory;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _clientUIPresenter = clientUIPresenter;
        _clientUIModel = clientUIModel;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _playerPresenter = playerPresenter;
        _keyboard = keyboard;
        _respawnAreaController = respawnAreaController;
    }

    public void Start()
    {
        GameObject player = _lobbyObjectFactory.CreatePlayer();
        var playerReferences = player.GetComponentInChildren<PlayerReferences>(true);

        _keyboard.playerRoot = playerReferences.Origin;
        _keyboard.cameraTransform = playerReferences.Camera;

        _respawnAreaController.Initialize(playerReferences.Origin, WorldID.None);

        GameObject[] mirrorObjects = GameObject.FindGameObjectsWithTag("Mirror");
        if (mirrorObjects.Length != 0)
        {
            foreach (var mirror in mirrorObjects)
            {
                mirror.GetComponent<MirrorView>().PlayerCamera = playerReferences.Camera;
            }
        }

        GameObject clientUI = _lobbyObjectFactory.CreateClientUI();
        clientUI.SetActive(false);
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>());
        _clientUIModel.Initialize(playerReferences.LeftHand);

        WorldUIView worldUIView = GameObject.FindAnyObjectByType<WorldUIView>(FindObjectsInactive.Include);
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(worldUIView);

        ProfileUIView profileUIView = GameObject.FindAnyObjectByType<ProfileUIView>(FindObjectsInactive.Include);
        _profileUIPresenter.Initialize(profileUIView);

        _playerPresenter.Initialize(player.GetComponentInChildren<PlayerView>(true));

        playerReferences.RightNearFarProfileFilter.ClientUI = clientUI;
    }

}
