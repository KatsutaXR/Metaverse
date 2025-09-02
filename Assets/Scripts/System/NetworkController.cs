using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// プロジェクト全体でのネットワーク処理を行う(プレイヤー生成などは行わない)
/// </summary>
public class NetworkController : INetworkRunnerCallbacks
{
    private readonly NetworkRunner _runner;

    private List<SessionInfo> _sessionList;

    [Inject]
    public NetworkController(NetworkRunner runner)
    {
        _runner = runner;
    }

    public void Initialize()
    {
        _runner.AddCallbacks(this);
    }

    /// <summary>
    /// Lobbyに参加するときに呼ばれる
    /// Lobbyに参加していないと各セッションの情報が確認できない
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinLobbyAsync()
    {
        var result = await _runner.JoinSessionLobby(SessionLobby.Shared);
    }

    /// <summary>
    /// Worldに参加するときに呼ばれる
    /// LobbyやWorldシーンから参照される
    /// todo:引数を受け取り、その値で参加の条件等を決める
    /// </summary>
    public async UniTask JoinWorldAsync()
    {
        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                {"WorldID", (int)WorldID.TestWorld}
            }
        });
    }

    /// <summary>
    /// Worldから退室する
    /// </summary>
    public async UniTask LeaveWorldAsync()
    {
        if (_runner != null) await _runner.Shutdown();
        else Debug.LogWarning("Runner is not assigned.");
    }

    /// <summary>
    /// 対象のワールドのセッションリストを返す関数
    /// "WorldDetail"で使用する
    /// </summary>
    /// <param name="targetWorldID"></param>
    /// <returns></returns>
    public List<SessionInfo> FindTargetSessions(WorldID targetWorldID)
    {
        Debug.Log($"FindTargetSessions, WorldID = {targetWorldID}");
        List<SessionInfo> targetSessionInfos = new List<SessionInfo>();

        if (_sessionList == null || _sessionList.Count == 0) return targetSessionInfos;

        foreach (SessionInfo sessionInfo in _sessionList)
        {
            if (!sessionInfo.Properties.TryGetValue("WorldID", out var sessionProperty)) continue;
            if (sessionProperty.GetType() != typeof(int)) continue;
            if ((WorldID)sessionProperty.PropertyValue == targetWorldID) targetSessionInfos.Add(sessionInfo);
        }

        return targetSessionInfos;
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("PlayerJoined");
    }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session List Updated:{sessionList}");
        _sessionList = sessionList;
    }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }

}
