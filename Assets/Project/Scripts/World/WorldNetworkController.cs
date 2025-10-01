using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
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
    protected WorldObjectFactory _worldObjectFactory;
    protected ClientUIPresenter _clientUIPresenter;
    protected ClientUIModel _clientUIModel;
    protected WorldUIPresenter _worldUIPresenter;
    protected ProfileUIPresenter _profileUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected ProfileStorage _profileStorage;
    protected GlobalNonNativeKeyboard _keyboard;

    protected WorldNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter, ProfileStorage profileStorage, GlobalNonNativeKeyboard keyboard)
    {
        _runner = runnerController.Runner;
        _respawnAreaController = respawnAreaController;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _worldObjectFactory = worldObjectFactory;
        _clientUIPresenter = clientUIPresenter;
        _clientUIModel = clientUIModel;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _playerPresenter = playerPresenter;
        _profileStorage = profileStorage;
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

        NetworkObject syncedAvatar = _worldObjectFactory.CreateSyncedAvatar(_runner, _runner.LocalPlayer);
        syncedAvatar.GetComponentInChildren<SyncedPlayerAvatar>(true).Initialize(playerReferences);
        SyncPlayerProfile syncPlayerProfile = syncedAvatar.GetComponentInChildren<SyncPlayerProfile>(true);
        syncPlayerProfile.Initialize(_profileStorage);
        var clientUI = SetupClientUI(playerReferences, syncedAvatar.GetComponentInChildren<SyncPlayerProfile>(true));

        _playerPresenter.Initialize(playerObject.GetComponentInChildren<PlayerView>(true));

        playerReferences.RightNearFarProfileFilter.ClientUI = clientUI;
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

    protected GameObject SetupClientUI(PlayerReferences playerReferences, SyncPlayerProfile syncPlayerProfile)
    {
        GameObject clientUI = _worldObjectFactory.CreateClientUI();
        clientUI.SetActive(false);
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>());
        _clientUIModel.Initialize(playerReferences.LeftHand);

        ProfileUIView profileUIView = GameObject.FindAnyObjectByType<ProfileUIView>(FindObjectsInactive.Include);
        _profileUIPresenter.Initialize(profileUIView, syncPlayerProfile);

        return clientUI;
        // WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
        // worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        // _worldUIPresenter.Initialize(worldUIView);
    }

    protected void SetupStroke(PlayerRef player)
    {
        var strokeManagers = GameObject.FindObjectsByType<StrokeController>(FindObjectsSortMode.None);
        if (strokeManagers.Length != 0)
        {
            foreach (var manager in strokeManagers)
            {
                manager.SyncStrokes(player);
            }
        }
    }

    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SetupStroke(player);
    }
    public virtual void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
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
    public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public virtual void OnSceneLoadDone(NetworkRunner runner) { }
    public virtual void OnSceneLoadStart(NetworkRunner runner) { }

    public abstract void Initialize();
    public abstract void Dispose();
}
