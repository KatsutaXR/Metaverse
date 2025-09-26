using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

/// <summary>
/// 各ワールド共通のネットワーク周りの処理を記載する
/// 各ワールドのNetworkControllerで継承して使用する
/// 用途に応じてoverrideする
/// </summary>
public abstract class WorldNetworkController : INetworkRunnerCallbacks, IDisposable
{
    protected NetworkRunner _runner;
    protected PrefabDatabase _prefabDatabase;
    protected WorldDatabase _worldDatabase;
    protected WorldObjectFactory _worldObjectFactory;
    protected ClientUIPresenter _clientUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected WorldUIPresenter _worldUIPresenter;
    protected PlayerData _playerData;

    public virtual void InitializeBase()
    {
        _runner.AddCallbacks(this);

        GameObject playerObject = _worldObjectFactory.CreatePlayer(_runner, _runner.LocalPlayer);
        _playerData.Player = playerObject;
        var playerReferences = playerObject.GetComponentInChildren<PlayerReferences>(true);

        // 鏡の設定
        SetupMirror(playerReferences);
        SetupClientUI(playerReferences);

        _playerPresenter.Initialize(playerObject.GetComponentInChildren<PlayerView>(true));
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

    // todo:最初は非活性にする
    protected void SetupClientUI(PlayerReferences playerReferences)
    {
        GameObject clientUI = _worldObjectFactory.CreateClientUI();
        _clientUIPresenter.Initialize(clientUI.GetComponent<ClientUIView>(), playerReferences);
        WorldUIView worldUIView = clientUI.GetComponentInChildren<WorldUIView>(true);
        worldUIView.CreateWorldListItems(_worldDatabase.Worlds.ToArray(), _prefabDatabase.WorldListItemPrefab);
        _worldUIPresenter.Initialize(worldUIView);
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
