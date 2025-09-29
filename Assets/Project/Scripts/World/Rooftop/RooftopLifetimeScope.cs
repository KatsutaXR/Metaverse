using VContainer;
using VContainer.Unity;

public class RooftopLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<RooftopInitializer>(Lifetime.Scoped).As<WorldInitializer>();
        builder.RegisterEntryPoint<RooftopMediator>(Lifetime.Scoped).As<WorldMediator>();

        builder.Register<RooftopNetworkController>(Lifetime.Scoped).As<WorldNetworkController>();
        builder.Register<RooftopObjectFactory>(Lifetime.Scoped).As<WorldObjectFactory>();
        builder.Register<RooftopClientUIModelUseCase>(Lifetime.Scoped).As<ClientUIModelUseCase>();
    }

}
