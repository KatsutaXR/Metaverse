using VContainer;

/// <summary>
/// Rooftop参加時の初期化処理用クラス
/// </summary>
public class RooftopInitializer : WorldInitializer
{
    public override WorldID TargetWorldID => WorldID.Rooftop;
    
    [Inject]
    public RooftopInitializer(WorldNetworkController worldNetworkController, NetworkController networkController, WorldDatabase worldDatabase) :
    base (worldNetworkController, networkController, worldDatabase) {}
    public override void Initialize()
    {
        InitializeBase();
    }
}
