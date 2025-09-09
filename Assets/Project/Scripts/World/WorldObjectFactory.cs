using Fusion;
using UnityEngine;

public abstract class WorldObjectFactory
{
    protected PrefabDatabase _prefabDatabase;
    public abstract Vector3 SpawnPlayerPosition { get; }
    public abstract Quaternion SpawnPlayerRotation { get; }

    public virtual GameObject CreatePlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        GameObject player = Object.Instantiate(_prefabDatabase.PlayerPrefabForWorld, SpawnPlayerPosition, SpawnPlayerRotation);

        NetworkObject syncedAvatar = runner.Spawn(_prefabDatabase.SyncedPlayerPrefab, SpawnPlayerPosition, SpawnPlayerRotation, playerRef);
        syncedAvatar.GetComponentInChildren<SyncedPlayerAvatar>(true).Initialize(player.GetComponentInChildren<AvatarData>(true));

        return player;
    }

    public virtual GameObject CreateClientUI()
    {
        GameObject clientUI = Object.Instantiate(_prefabDatabase.ClientUIPrefab);
        return clientUI;
    }
}
