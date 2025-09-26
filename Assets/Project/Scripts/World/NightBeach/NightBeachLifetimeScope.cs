using VContainer;
using VContainer.Unity;

public class NightBeachLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<NightBeachInitializer>(Lifetime.Scoped).As<WorldInitializer>();
        builder.RegisterEntryPoint<NightBeachMediator>(Lifetime.Scoped).As<WorldMediator>();

        builder.Register<NightBeachNetworkController>(Lifetime.Scoped).As<WorldNetworkController>();
        builder.Register<NightBeachObjectFactory>(Lifetime.Scoped).As<WorldObjectFactory>();
        builder.Register<NightBeachClientUIModelUseCase>(Lifetime.Scoped).As<ClientUIModelUseCase>();
    }

}
