using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

/// <summary>
/// 各ワールド共通のネットワーク周りの処理を記載する
/// 各ワールドのNetworkControllerで継承して使用する
/// 用途に応じてoverrideする
/// </summary>
public abstract class WorldNetworkController : INetworkRunnerCallbacks, IDisposable
{
    protected NetworkRunner _runner;
    protected RespawnAreaController _respawnAreaController;
    protected PrefabDatabase _prefabDatabase;
    protected WorldDatabase _worldDatabase;
    protected AvatarDatabase _avatarDatabase;
    protected WorldObjectFactory _worldObjectFactory;
    protected ClientUIPresenter _clientUIPresenter;
    protected ClientUIModel _clientUIModel;
    protected WorldUIPresenter _worldUIPresenter;
    protected ProfileUIPresenter _profileUIPresenter;
    protected AvatarUIPresenter _avatarUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected ProfileStorage _profileStorage;
    protected PlayerXRUtility _playerXRUtility;
    protected GlobalNonNativeKeyboard _keyboard;

    protected WorldNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, AvatarDatabase avatarDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter, ProfileStorage profileStorage, PlayerXRUtility playerXRUtility, GlobalNonNativeKeyboard keyboard)
    {
        _runner = runnerController.Runner;
        _respawnAreaController = respawnAreaController;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _avatarDatabase = avatarDatabase;
        _worldObjectFactory = worldObjectFactory;
        _clientUIPresenter = clientUIPresenter;
        _clientUIModel = clientUIModel;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _avatarUIPresenter = avatarUIPresenter;
        _playerPresenter = playerPresenter;
        _profileStorage = profileStorage;
        _playerXRUtility = playerXRUtility;
        _keyboard = keyboard;
    }

    public virtual void InitializeBase()
    {
        _runner.AddCallbacks(this);

        GameObject playerObject = _worldObjectFactory.CreatePlayer();
        var playerReferences = playerObject.GetComponentInChildren<PlayerReferences>(true);

        _keyboard.playerRoot = playerReferences.Origin;
        _keyboard.cameraTransform = playerReferences.Camera;

        _respawnAreaController.Initialize(playerReferences.Origin, _worldObjectFactory.TargetWorldID);

        // 鏡の設定
        SetupMirror(playerReferences);

        // World特有処理
        NetworkObject syncPlayerRootObj = _worldObjectFactory.CreateSyncPlayerRoot(_runner, _runner.LocalPlayer);
        _runner.SetPlayerObject(_runner.LocalPlayer, syncPlayerRootObj);
        SyncPlayerRoot syncPlayerRoot = syncPlayerRootObj.GetComponent<SyncPlayerRoot>();
        syncPlayerRoot.Initialize(playerReferences);
        syncPlayerRootObj.GetComponentInChildren<SyncPlayerAvatar>(true).Initialize(playerReferences);
        SyncPlayerProfile syncPlayerProfile = syncPlayerRoot.GetComponentInChildren<SyncPlayerProfile>(true);
        syncPlayerProfile.Initialize(_profileStorage);

        var clientUI = SetupClientUI(playerReferences, syncPlayerProfile, syncPlayerRoot);

        _playerPresenter.Initialize(playerObject.GetComponentInChildren<PlayerView>(true));

        playerReferences.RightNearFarProfileFilter.ClientUI = clientUI;

        _playerXRUtility.WaitTrackingStartAndRecenter().Forget();
    }

    protected void SetupMirror(PlayerReferences playerReferences)
    {
        var mirrors = GameObject.FindObjectsByType<MirrorView>(FindObjectsSortMode.None);
        if (mirrors.Length != 0)
        {
            foreach (var mirror in mirrors)
            {
                mirror.PlayerCamera = playerReferences.Camera;
            }
        }
    }

    protected GameObject SetupClientUI(PlayerReferences playerReferences, SyncPlayerProfile syncPlayerProfile, SyncPlayerRoot syncPlayerRoot)
    {
        GameObject clientUI = _worldObjectFactory.CreateClientUI();
        clientUI.SetActive(false);
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>());
        _clientUIModel.Initialize(playerReferences);

        ProfileUIView profileUIView = GameObject.FindAnyObjectByType<ProfileUIView>(FindObjectsInactive.Include);
        // WorldではSyncPlayerProfileを渡す
        _profileUIPresenter.Initialize(profileUIView, syncPlayerProfile);

        AvatarUIView avatarUIView = GameObject.FindAnyObjectByType<AvatarUIView>(FindObjectsInactive.Include);
        avatarUIView.Initialize(_avatarDatabase.Avatars.ToArray(), _prefabDatabase.AvatarListItemPrefab);
        // WorldではSyncPlayerRootを渡す
        _avatarUIPresenter.Initialize(avatarUIView, syncPlayerRoot: syncPlayerRoot);

        return clientUI;
    }

    protected void SetupStroke(PlayerRef targetPlayer)
    {
        var controllers = GameObject.FindObjectsByType<StrokeController>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                controller.SyncStrokes(targetPlayer);
            }
        }
    }

    protected void SetupAvatar(PlayerRef targetPlayer)
    {
        var controllers = GameObject.FindObjectsByType<SyncPlayerAvatar>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                if (controller.HasStateAuthority)
                {
                    controller.SyncAvatar(targetPlayer);
                    // 権限を持つアバターは一つしかないのでbreak
                    break;
                }
            }
        }
    }

    protected void SetupPhotoDisplay(PlayerRef targetPlayer)
    {
        var controllers = GameObject.FindObjectsByType<PhotoDisplayController>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                Debug.Log("SetupPhotoDisplay");
                controller.SyncPhoto(targetPlayer);
            }
        }
    }

    protected async UniTask UpdateNumberOfPeopleInSession()
    {
        // マスタークライアントの変更を待つ
        await UniTask.WaitForSeconds(1);
        if (!_runner.IsSharedModeMasterClient) return;

        var controllers = GameObject.FindObjectsByType<SessionInfoUIController>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                controller.UpdateNumberOfPeople();
            }
        }
    }

    protected void SetupSessionInfoItem(PlayerRef targetPlayer)
    {
        // 重たい処理になる場合は状態権限を持つ各クライアントに任せる
        if (!_runner.IsSharedModeMasterClient) return;

        var controllers = GameObject.FindObjectsByType<SessionInfoItemController>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                controller.RpcSyncSessionInfoItem(targetPlayer);
            }
        }
    }

    protected async UniTask RequestSpawnLeaveSessionInfoItem(PlayerRef targetPlayer)
    {
        // マスタークライアントの変更を待つ
        await UniTask.WaitForSeconds(1);
        if (!_runner.IsSharedModeMasterClient) return;

        var controllers = GameObject.FindObjectsByType<SessionInfoUIController>(FindObjectsSortMode.None);
        if (controllers.Length != 0)
        {
            foreach (var controller in controllers)
            {
                controller.SpawnLeaveSessionInfoItem(targetPlayer);
            }
        }
    }

    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SetupStroke(player);
        SetupAvatar(player);
        SetupPhotoDisplay(player);
        UpdateNumberOfPeopleInSession().Forget();
        SetupSessionInfoItem(player);
    }

    /// <summary>
    /// マスタークライアントが退出した時はその変更を待ってから処理を行う必要がある
    /// 強制終了などでは挙動がおかしくなるため一旦数秒待つことで対処する
    /// </summary>
    public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
        UpdateNumberOfPeopleInSession().Forget();
        RequestSpawnLeaveSessionInfoItem(player).Forget();
    }

    /// <summary>
    /// Runner.SendReliableDataToPlayerのデータがすべて届いたときに呼ばれる関数
    /// todo:写真の同期以外で使う場合はロジックを変更する必要あり
    /// </summary>
    public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        key.GetInts(out int key0, out _, out _, out _);
        NetworkId networkId = new NetworkId();
        networkId.Raw = (uint)key0;
        NetworkObject obj = _runner.FindObject(networkId);
        if (obj == null) return;
        obj.GetComponent<PhotoDisplayController>().ApplyTextureFromBytes(data.ToArray());
    }

    public virtual void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnInput(NetworkRunner runner, NetworkInput input) { }
    public virtual void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public virtual void OnConnectedToServer(NetworkRunner runner) { }
    public virtual void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public virtual void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public virtual void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public virtual void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public virtual void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public virtual void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public virtual void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public virtual void OnSceneLoadDone(NetworkRunner runner) { }
    public virtual void OnSceneLoadStart(NetworkRunner runner) { }

    public abstract void Initialize();
    public abstract void Dispose();
}
