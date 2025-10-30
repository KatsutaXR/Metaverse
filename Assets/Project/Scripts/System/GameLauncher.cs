using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ゲーム実行時の処理を行うクラス
/// </summary>
public class GameLauncher : IStartable
{
    private readonly NetworkRunnerController _networkRunnerController;
    private readonly NetworkController _networkController;
    [Inject]
    public GameLauncher(NetworkRunnerController networkRunnerController, NetworkController networkController)
    {
        _networkRunnerController = networkRunnerController;
        _networkController = networkController;
    }

    public void Start()
    {
        // 初期化処理
        _networkRunnerController.Initialize();
        _networkController.Initialize();

        _networkController.JoinLobbyAsync().Forget();
    }
}
