using VContainer;

/// <summary>
/// NightBeach参加時の初期化処理用クラス
/// </summary>
public class NightBeachInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.NightBeach;

    [Inject]
    public NightBeachInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase) :
    base (worldNetworkController, networkController, worldDatabase) {}

    public override void Initialize()
    {
        InitializeBase();
    }
}
