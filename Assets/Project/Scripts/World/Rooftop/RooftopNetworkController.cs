using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using VContainer;

public class RooftopNetworkController : WorldNetworkController
{
    [Inject]
    public RooftopNetworkController(NetworkRunnerController runnerController, RespawnAreaController respawnAreaController, PrefabDatabase prefabDatabase, WorldDatabase worldDatabase, AvatarDatabase avatarDatabase, WorldObjectFactory worldObjectFactory, ClientUIPresenter clientUIPresenter, ClientUIModel clientUIModel, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter, ProfileStorage profileStorage, PlayerXRUtility playerXRUtility, GlobalNonNativeKeyboard keyboard) :
    base(runnerController, respawnAreaController, prefabDatabase, worldDatabase, avatarDatabase, worldObjectFactory, clientUIPresenter, clientUIModel, worldUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter, profileStorage, playerXRUtility, keyboard) {}
    
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
