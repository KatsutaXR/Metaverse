using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using VContainer;
using VContainer.Unity;

public class SystemLifetimeScope : LifetimeScope
{
    [SerializeField] private NetworkRunner _networkRunner;
    [SerializeField] private NetworkRunnerController _networkRunnerController;
    [SerializeField] private GlobalNonNativeKeyboard _keyboard;
    [SerializeField] private WorldDatabase _worldDatabase;
    [SerializeField] private PrefabDatabase _prefabDatabase;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_networkRunner);
        builder.RegisterComponent(_keyboard);
        builder.RegisterInstance(_worldDatabase);
        builder.RegisterInstance(_prefabDatabase);

        builder.RegisterInstance(_networkRunnerController);
        
        builder.Register<NetworkController>(Lifetime.Singleton);
        builder.Register<SceneController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GameLauncher>(Lifetime.Singleton);
    }
}
