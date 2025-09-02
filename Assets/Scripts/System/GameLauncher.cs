using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ゲーム実行時の処理を行うクラス
/// 他のクラスの初期化も行う
/// todo:プレイヤー設定の読み込み、実行時Lobbyシーンへの遷移などを行いたい
/// </summary>
public class GameLauncher : IStartable
{
    private readonly NetworkRunnerController _networkRunnerController;
    private readonly NetworkController _networkController;
    private readonly SceneController _sceneController;
    [Inject]
    public GameLauncher(NetworkRunnerController networkRunnerController, NetworkController networkController, SceneController sceneController)
    {
        _networkRunnerController = networkRunnerController;
        _networkController = networkController;
        _sceneController = sceneController;
    }

    // todo:修正
    public void Start()
    {
        // 初期化処理
        _networkRunnerController.Initialize();
        _networkController.Initialize(); // todo:networkRunnercontrollerに対応させる

        // テスト
        _networkController.JoinLobbyAsync().Forget();
        //_sceneController.LoadWorldAsync(WorldID.TestWorld).Forget();
        //_sceneController.LoadLobbyAsync().Forget();
    }
}
