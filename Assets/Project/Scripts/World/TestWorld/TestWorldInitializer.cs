using VContainer;

/// <summary>
/// TestWorld参加時の初期化処理用クラス
/// </summary>
public class TestWorldInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.TestWorld;

    [Inject]
    public TestWorldInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase) :
    base (worldNetworkController, networkController, worldDatabase) {}

    public override void Initialize()
    {
        InitializeBase();
    }
}
