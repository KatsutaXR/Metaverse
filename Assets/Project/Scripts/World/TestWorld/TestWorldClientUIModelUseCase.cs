using UnityEngine;

public class TestWorldClientUIModelUseCase : IClientUIModelUseCase
{
    // todo:リスポーン位置の修正
    private Vector3 _respawnPosition = Vector3.one;
    public Vector3 Respawn()
    {
        return _respawnPosition;
    }
}
