using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SystemLifetimeScope : LifetimeScope
{
    [SerializeField] private NetworkRunner _networkRunner;
    [SerializeField] private WorldDatabase _worldDatabase;
    [SerializeField] private PrefabDatabase _prefabDatabase;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_networkRunner);
        builder.RegisterInstance(_worldDatabase);
        builder.RegisterInstance(_prefabDatabase);
        
        builder.Register<NetworkController>(Lifetime.Singleton);
        builder.Register<SceneController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<GameLauncher>(Lifetime.Singleton);
    }
}
