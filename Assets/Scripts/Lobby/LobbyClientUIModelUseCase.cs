using UnityEngine;

public class LobbyClientUIModelUseCase : IClientUIModelUseCase
{
    private Vector3 _respawnPosition = Vector3.zero;
    public Vector3 Respawn()
    {
        return _respawnPosition;
    }
}
