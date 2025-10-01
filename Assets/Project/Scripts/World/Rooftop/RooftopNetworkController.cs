using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using VContainer;

public class RooftopNetworkController : WorldNetworkController
{
    [Inject]
    public RooftopNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter, ProfileStorage profileStorage, GlobalNonNativeKeyboard keyboard) :
    base(runnerController, respawnAreaController, prefabDatabase, worldDatabase, worldObjectFactory, clientUIPresenter, clientUIModel, worldUIPresenter, profileUIPresenter, playerPresenter, profileStorage, keyboard) {}
    
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
