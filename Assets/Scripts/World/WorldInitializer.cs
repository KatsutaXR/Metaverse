using VContainer.Unity;

/// <summary>
/// 各ワールドで行う共通の初期化処理をまとめるクラス
/// 各ワールドの初期化用クラスで継承して使う
/// </summary>
public abstract class WorldInitializer : IStartable
{
    public abstract void Start();
}
