using VContainer;
using VContainer.Unity;

public class LobbyLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<LobbyInitializer>(Lifetime.Scoped);
        builder.RegisterEntryPoint<LobbyMediator>(Lifetime.Scoped);
        builder.Register<LobbyObjectFactory>(Lifetime.Scoped);

        // ClientUI
        builder.Register<ClientUIPresenter>(Lifetime.Scoped);
        builder.Register<ClientUIModel>(Lifetime.Scoped);
        builder.Register<LobbyClientUIModelUseCase>(Lifetime.Scoped).As<ClientUIModelUseCase>();

        // WorldUI
        builder.Register<WorldUIPresenter>(Lifetime.Scoped);
        builder.Register<WorldUIModel>(Lifetime.Scoped);

        // ProfileUI
        builder.Register<ProfileUIPresenter>(Lifetime.Scoped);
        builder.Register<ProfileUIModel>(Lifetime.Scoped);

        // AvatarUI
        builder.Register<AvatarUIPresenter>(Lifetime.Scoped);
        builder.Register<AvatarUIModel>(Lifetime.Scoped);

        // Player
        builder.Register<PlayerModel>(Lifetime.Scoped);
        builder.Register<PlayerPresenter>(Lifetime.Scoped);
    }
}
