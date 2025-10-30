using VContainer;

/// <summary>
/// Theater参加時の初期化処理用クラス
/// </summary>
public class TheaterInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.Theater;

    [Inject]
    public TheaterInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase) :
    base (worldNetworkController, networkController, worldDatabase) {}

    public override void Initialize()
    {
        InitializeBase();
    }
}
