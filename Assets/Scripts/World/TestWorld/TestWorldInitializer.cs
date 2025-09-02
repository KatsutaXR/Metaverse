using UnityEngine;
using VContainer;

/// <summary>
/// TestWorld参加時の初期化処理用クラス
/// </summary>
public class TestWorldInitializer : WorldInitializer
{
    private TestWorldNetworkController _testWorldNetworkController;
    [Inject]
    public TestWorldInitializer(TestWorldNetworkController testWorldNetworkController)
    {
        _testWorldNetworkController = testWorldNetworkController;
    }
    public override void Start()
    {
        // 初期化処理
        _testWorldNetworkController.Initialize();
    }
}
