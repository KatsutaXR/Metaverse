using NUnit.Framework.Internal;
using VContainer;
using VContainer.Unity;

public class TestWorldLifetimeScope : WorldLifetimeScope
{
    protected override void ConfigureWorldSpecificDependencies(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<TestWorldInitializer>(Lifetime.Scoped);
        builder.RegisterEntryPoint<TestWorldMediator>(Lifetime.Scoped);
        builder.Register<TestWorldNetworkController>(Lifetime.Scoped);
        builder.Register<TestWorldObjectFactory>(Lifetime.Scoped);

        builder.Register<IClientUIModelUseCase, TestWorldClientUIModelUseCase>(Lifetime.Scoped);
    }

}
