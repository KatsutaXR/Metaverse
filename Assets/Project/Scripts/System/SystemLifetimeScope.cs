using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;
using VContainer;
using VContainer.Unity;

public class SystemLifetimeScope : LifetimeScope
{
    [SerializeField] private NetworkRunnerController _networkRunnerController;
    [SerializeField] private RespawnAreaController _respawnAreaController;
    [SerializeField] private GlobalNonNativeKeyboard _keyboard;
    [SerializeField] private WorldDatabase _worldDatabase;
    [SerializeField] private PrefabDatabase _prefabDatabase;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_keyboard);
        builder.RegisterInstance(_worldDatabase);
        builder.RegisterInstance(_prefabDatabase);

        builder.RegisterComponent(_networkRunnerController);
        builder.RegisterComponent(_respawnAreaController);
        
        builder.Register<RecorderController>(Lifetime.Singleton);
        builder.Register<NetworkController>(Lifetime.Singleton);
        builder.Register<SceneController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GameLauncher>(Lifetime.Singleton);
    }
}
