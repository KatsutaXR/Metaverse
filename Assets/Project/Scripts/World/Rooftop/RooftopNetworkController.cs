using VContainer;

public class RooftopNetworkController : WorldNetworkController
{
    [Inject]
    public RooftopNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter, PlayerData playerData)
    {
        _runner = runnerController.Runner;
        _respawnAreaController = respawnAreaController;
        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
        _worldObjectFactory = worldObjectFactory;
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _playerData = playerData;
    }

    /// <summary>
    /// ワールド内の初期化処理を行う
    /// 同期オブジェクトに関しては基本各Spawned内で初期化させる
    /// </summary>
    public override void Initialize()
    {
        InitializeBase();
    }

    public override void Dispose()
    {
        _runner.RemoveCallbacks(this);
    }
}
