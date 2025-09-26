using VContainer;
using VContainer.Unity;

public class SimpleRoomLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<SimpleRoomInitializer>(Lifetime.Scoped).As<WorldInitializer>();
        builder.RegisterEntryPoint<SimpleRoomMediator>(Lifetime.Scoped).As<WorldMediator>();

        builder.Register<SimpleRoomNetworkController>(Lifetime.Scoped).As<WorldNetworkController>();
        builder.Register<SimpleRoomObjectFactory>(Lifetime.Scoped).As<WorldObjectFactory>();
        builder.Register<SimpleRoomClientUIModelUseCase>(Lifetime.Scoped).As<ClientUIModelUseCase>();
    }

}
