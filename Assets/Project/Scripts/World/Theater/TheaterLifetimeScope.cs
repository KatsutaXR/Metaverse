using VContainer;
using VContainer.Unity;

public class TheaterLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<TheaterInitializer>(Lifetime.Scoped).As<WorldInitializer>();
        builder.RegisterEntryPoint<TheaterMediator>(Lifetime.Scoped).As<WorldMediator>();

        builder.Register<TheaterNetworkController>(Lifetime.Scoped).As<WorldNetworkController>();
        builder.Register<TheaterObjectFactory>(Lifetime.Scoped).As<WorldObjectFactory>();
        builder.Register<TheaterClientUIModelUseCase>(Lifetime.Scoped).As<ClientUIModelUseCase>();
    }

}
