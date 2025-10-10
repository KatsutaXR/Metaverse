using System.Linq;
using Cysharp.Threading.Tasks;
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
    private AvatarDatabase _avatarDatabase;
    private ClientUIPresenter _clientUIPresenter;
    private ClientUIModel _clientUIModel;
    private WorldUIPresenter _worldUIPresenter;
    private ProfileUIPresenter _profileUIPresenter;
    private AvatarUIPresenter _avatarUIPresenter;
    private PlayerPresenter _playerPresenter;
    private PlayerXRUtility _playerXRUtility;
    private GlobalNonNativeKeyboard _keyboard;
    private RespawnAreaController _respawnAreaController;
    [Inject]
    public LobbyInitializer(LobbyObjectFactory lobbyObjectFactory, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, AvatarDatabase avatarDatabase, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter, PlayerXRUtility playerXRUtility, GlobalNonNativeKeyboard keyboard, RespawnAreaController respawnAreaController)
    {
        _lobbyObjectFactory = lobbyObjectFactory;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _avatarDatabase = avatarDatabase;
        _clientUIPresenter = clientUIPresenter;
        _clientUIModel = clientUIModel;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _avatarUIPresenter = avatarUIPresenter;
        _playerPresenter = playerPresenter;
        _playerXRUtility = playerXRUtility;
        _keyboard = keyboard;
        _respawnAreaController = respawnAreaController;
    }

    public void Start()
    {
        GameObject player = _lobbyObjectFactory.CreatePlayer();
        var playerReferences = player.GetComponentInChildren<PlayerReferences>(true);

        // Lobby特有処理
        AvatarView avatarView = playerReferences.AvatarView;
        avatarView.Initialize();

        // Lobby特有処理
        WorldUIView worldUIView = GameObject.FindAnyObjectByType<WorldUIView>(FindObjectsInactive.Include);
        worldUIView.Initialize(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(worldUIView);

        _keyboard.playerRoot = playerReferences.Origin;
        _keyboard.cameraTransform = playerReferences.Camera;

        _respawnAreaController.Initialize(playerReferences.Origin, WorldID.None);

        var mirrors = GameObject.FindObjectsByType<MirrorView>(FindObjectsSortMode.None);
        if (mirrors.Length != 0)
        {
            foreach (var mirror in mirrors)
            {
                mirror.PlayerCamera = playerReferences.Camera;
            }
        }

        GameObject clientUI = _lobbyObjectFactory.CreateClientUI();
        clientUI.SetActive(false);
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>());
        _clientUIModel.Initialize(playerReferences);

        ProfileUIView profileUIView = GameObject.FindAnyObjectByType<ProfileUIView>(FindObjectsInactive.Include);
        _profileUIPresenter.Initialize(profileUIView);

        AvatarUIView avatarUIView = GameObject.FindAnyObjectByType<AvatarUIView>(FindObjectsInactive.Include);
        avatarUIView.Initialize(_avatarDatabase.Avatars.ToArray(), _prefabDatabase.AvatarListItemPrefab);
        // LobbyではAvatarViewを渡す
        _avatarUIPresenter.Initialize(avatarUIView, avatarView: avatarView);

        _playerPresenter.Initialize(player.GetComponentInChildren<PlayerView>(true));

        playerReferences.RightNearFarProfileFilter.ClientUI = clientUI;

        _playerXRUtility.WaitTrackingStartAndRecenter().Forget();
    }

}
