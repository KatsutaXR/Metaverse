using Fusion;
using UnityEngine;

public abstract class WorldObjectFactory
{
    protected PrefabDatabase _prefabDatabase;
    protected NetworkController _networkController;
    protected WorldDatabase _worldDatabase;
    public abstract WorldID TargetWorldID { get; }

    protected WorldObjectFactory(PrefabDatabase prefabDatabase, NetworkController networkController, WorldDatabase worldDatabase)
    {
        _prefabDatabase = prefabDatabase;
        _networkController = networkController;
        _worldDatabase = worldDatabase;
    }

    public virtual GameObject CreatePlayer()
    {
        var worldData = _worldDatabase.GetWorldById(TargetWorldID);
        var spawnPosition = worldData.PlayerSpawnPosiion;
        var spawnRotation = worldData.PlayerSpawnRotation;

        GameObject player = Object.Instantiate(_prefabDatabase.PlayerPrefabForWorld, spawnPosition, spawnRotation);
        return player;
    }

    public virtual NetworkObject CreateSyncPlayerRoot(NetworkRunner runner, PlayerRef playerRef)
    {
        var worldData = _worldDatabase.GetWorldById(TargetWorldID);
        var spawnPosition = worldData.PlayerSpawnPosiion;
        var spawnRotation = worldData.PlayerSpawnRotation;

        NetworkObject syncPlayerRoot = runner.Spawn(_prefabDatabase.SyncPlayerRootPrefab, spawnPosition, spawnRotation, playerRef);
        return syncPlayerRoot;
    }

    public virtual GameObject CreateClientUI()
    {
        GameObject clientUI = Object.Instantiate(_prefabDatabase.ClientUIPrefab);
        return clientUI;
    }
}
