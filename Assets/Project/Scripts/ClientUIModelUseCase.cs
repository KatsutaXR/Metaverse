using UnityEngine;

public abstract class ClientUIModelUseCase
{
    protected WorldDatabase _worldDatabase;
    public abstract WorldID TargetWorldID { get; }
    public virtual (Vector3, Quaternion) Respawn()
    {
        switch (TargetWorldID)
        {
            case WorldID.None:
                // ロビーのリスポーン地点を返す
                return (Vector3.zero, Quaternion.identity);
            default:
                var worldData = _worldDatabase.GetWorldById(TargetWorldID);
                return (worldData.PlayerSpawnPosiion, worldData.PlayerSpawnRotation);
        }
    }
}
