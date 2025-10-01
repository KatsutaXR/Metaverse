using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using VContainer;

public class SimpleRoomNetworkController : WorldNetworkController
{
    [Inject]
    public SimpleRoomNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter, ProfileStorage profileStorage, GlobalNonNativeKeyboard keyboard)
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
