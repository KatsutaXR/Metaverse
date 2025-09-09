using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using VContainer;

/// <summary>
/// プロジェクト全体でのネットワーク処理を行う(プレイヤー生成などは行わない)
/// </summary>
public class NetworkController : INetworkRunnerCallbacks
{
    public const string WORLD_ID_KEY = "WorldID";
    public const string PASSWORD_KEY = "Password";
    private readonly NetworkRunnerController _networkRunnerController;
    private readonly SceneController _sceneController;

    private List<SessionInfo> _sessionList;
    private bool _isLobbyScene;
    private WorldID _currentWorldID;

    [Inject]
    public NetworkController(NetworkRunnerController networkRunnerController, SceneController sceneController)
    {
        _networkRunnerController = networkRunnerController;
        _sceneController = sceneController;

        _isLobbyScene = false;
        _currentWorldID = WorldID.None;
    }

    public void Initialize()
    {
        _networkRunnerController.Runner.AddCallbacks(this);
    }

    /// <summary>
    /// Lobbyに参加するときに呼ばれる
    /// Lobbyに参加していないと各セッションの情報が確認できない
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinLobbyAsync()
    {
        // 現在Lobbyシーンにいるなら早期リターン
        if (_isLobbyScene) return;

        if (_currentWorldID != WorldID.None)
        {
            await _sceneController.UnloadWorldAsync(_currentWorldID);

            // ランナーを新しくし、AddCallbacksで紐づける
            await _networkRunnerController.ShutdownRunner();
            _networkRunnerController.Runner.AddCallbacks(this);
        }
        

        await _sceneController.LoadLobbyAsync();
        await _networkRunnerController.JoinLSessionLobbyAsync();

        _isLobbyScene = true;
        _currentWorldID = WorldID.None;

    }

    public async UniTask CreateSessionAsync(CustomSessionInfo customSessionInfo)
    {
        // セッション名とWorldIDが一致するものがあればエラーUIにその旨を表示して終了
        foreach (var sessionInfo in _sessionList)
        {
            if (sessionInfo.Name != customSessionInfo.SessionName) continue;

            if (!sessionInfo.Properties.TryGetValue(WORLD_ID_KEY, out var sessionProperty)) continue;
            if (sessionProperty.PropertyType != typeof(int)) continue;
            if ((WorldID)sessionProperty.PropertyValue != customSessionInfo.WorldID) continue;

            // todo:エラーを表示させる
            return;
        }

        await _sceneController.UnloadLobbyAsync();
        await _sceneController.LoadWorldAsync(customSessionInfo.WorldID);

        await _networkRunnerController.StartSessionAsync(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = customSessionInfo.SessionName,
            PlayerCount = customSessionInfo.MaxPlayers,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                {WORLD_ID_KEY, (int)customSessionInfo.WorldID},
                {PASSWORD_KEY, customSessionInfo.Password}
            }
        });

        _isLobbyScene = false;
        _currentWorldID = customSessionInfo.WorldID;
    }

    public async UniTask JoinSessionAsync(CustomSessionInfo customSessionInfo)
    {
        if (_sessionList == null || _sessionList.Count == 0) return;

        SessionInfo targetSessionInfo = null;
        foreach (var sessionInfo in _sessionList)
        {
            if (sessionInfo.Name != customSessionInfo.SessionName) continue;

            if (!sessionInfo.Properties.TryGetValue(WORLD_ID_KEY, out var sessionProperty)) continue;
            if (sessionProperty.PropertyType != typeof(int)) continue;
            if ((WorldID)sessionProperty.PropertyValue != customSessionInfo.WorldID) continue;
            
            targetSessionInfo = sessionInfo;
            break;
        }

        if (targetSessionInfo == null)
        {
            // todo:対象のセッションがないことをエラーUIに出して終了
            Debug.LogError("TargetSession is null");
            return;
        }

        if (!targetSessionInfo.Properties.TryGetValue(PASSWORD_KEY, out var password) ||
            password.PropertyType != typeof(string) ||
            (string)password.PropertyValue != customSessionInfo.Password)
        {
            // todo:パスワードが違うとエラーUIに出して終了
            Debug.LogError("Password is not correct");
            return;
        }

        if (targetSessionInfo.PlayerCount >= targetSessionInfo.MaxPlayers)
        {
            // todo:セッションが満員であることをエラーUIに出して終了
            Debug.LogError("Session is full");
            return;
        }

        await _sceneController.UnloadLobbyAsync();
        await _sceneController.LoadWorldAsync(customSessionInfo.WorldID);

        await _networkRunnerController.StartSessionAsync(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = customSessionInfo.SessionName,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                {WORLD_ID_KEY, (int)customSessionInfo.WorldID },
                {PASSWORD_KEY, customSessionInfo.Password }
            }
        });

        _isLobbyScene = false;
        _currentWorldID = customSessionInfo.WorldID;

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
            if (!sessionInfo.Properties.TryGetValue(WORLD_ID_KEY, out var sessionProperty)) continue;
            if (sessionProperty.PropertyType != typeof(int)) continue;
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
