using VContainer;
using VContainer.Unity;
using UnityEngine;

/// <summary>
/// 各World共通の依存関係を定義する
/// 各WorldのLifetimeScopeで継承して使用する
/// </summary>
public abstract class WorldLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ClientUIModel>(Lifetime.Scoped);
        builder.Register<ClientUIPresenter>(Lifetime.Scoped);
        builder.Register<WorldUIPresenter>(Lifetime.Scoped);
        builder.Register<WorldUIModel>(Lifetime.Scoped);

        builder.Register<PlayerModel>(Lifetime.Scoped);
        builder.Register<PlayerPresenter>(Lifetime.Scoped);
        builder.Register<PlayerData>(Lifetime.Scoped);

        // 各World固有の依存関係を注入する
        ConfigureWorldSpecificDependencies(builder);
    }

    protected virtual void ConfigureWorldSpecificDependencies(IContainerBuilder builder) {}
}
