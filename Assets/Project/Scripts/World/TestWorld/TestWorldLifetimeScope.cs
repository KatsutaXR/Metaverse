using VContainer;
using VContainer.Unity;

public class TestWorldLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<TestWorldInitializer>(Lifetime.Scoped).As<WorldInitializer>();
        builder.RegisterEntryPoint<TestWorldMediator>(Lifetime.Scoped).As<WorldMediator>();

        builder.Register<TestWorldNetworkController>(Lifetime.Scoped).As<WorldNetworkController>();
        builder.Register<TestWorldObjectFactory>(Lifetime.Scoped).As<WorldObjectFactory>();

        builder.Register<IClientUIModelUseCase, TestWorldClientUIModelUseCase>(Lifetime.Scoped);
    }

}
