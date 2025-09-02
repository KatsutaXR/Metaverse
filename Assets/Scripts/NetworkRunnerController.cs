using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using VContainer;

public class NetworkRunnerController : MonoBehaviour
{
    private NetworkRunner _runner;
    public NetworkRunner Runner => _runner;
    [Inject] private PrefabDatabase _prefabDatabase;

    /// <summary>
    /// ネットワークランナーを生成して他から参照する
    /// プロジェクト起動時の最初に呼ぶ必要がある
    /// </summary>
    public void Initialize()
    {
        var runnerPrefab = _prefabDatabase.NetworkRunnerPrefab;
        _runner = Instantiate(runnerPrefab).GetComponent<NetworkRunner>();
    }

    public async UniTask JoinLobbyAsync()
    {
        await _runner.JoinSessionLobby(SessionLobby.Shared);
    }

    public async UniTask StartSessionAsync(StartGameArgs args)
    {
        await _runner.StartGame(args);
    }

    /// <summary>
    /// ランナーをシャットダウンする
    /// シャットダウン時にランナーを破壊、生成を行う(シーンロード時にランナーを使用するため先に作る必要がある)
    /// AddCallbacksで紐づけられていたクラスを再度紐づける必要がある
    /// </summary>
    /// <returns></returns>
    public async UniTask ShutdownRunner()
    {
        if (_runner == null) return;

        await _runner.Shutdown();
        Destroy(_runner.gameObject);
        _runner = null;

        // 1フレーム待つと安定しやすいらしい
        await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

        var runnerPrefab = _prefabDatabase.NetworkRunnerPrefab;
        _runner = Instantiate(runnerPrefab).GetComponent<NetworkRunner>();
    }
}
