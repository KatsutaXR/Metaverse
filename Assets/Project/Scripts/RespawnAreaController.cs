using System;
using UniRx;
using UnityEngine;
using VContainer;

public class RespawnAreaController : MonoBehaviour
{
    [Inject] private WorldDatabase _worldDatabase;
    public Transform TargetPlayer { get; set; }
    private Vector3 _respawnPosition;
    private Quaternion _respawnRotation;
    private const float Limit_Y_Position = -50f;
    private readonly Subject<(Vector3, Quaternion)> _limitYPositionReached = new Subject<(Vector3, Quaternion)>();
    public IObservable<(Vector3, Quaternion)> LimitYPositionReached => _limitYPositionReached;

    public void Initialize(Transform targetPlayer, WorldID targetWorldID)
    {
        Debug.Log($"targetPlayer = {targetPlayer}, targetWorldID = {targetWorldID}");
        TargetPlayer = targetPlayer;

        switch (targetWorldID)
        {
            case WorldID.None:
                // ロビーのリスポーン設定をする
                _respawnPosition = Vector3.zero;
                _respawnRotation = Quaternion.identity;
                break;
            default:
                var worldData = _worldDatabase.GetWorldById(targetWorldID);
                _respawnPosition = worldData.PlayerSpawnPosiion;
                _respawnRotation = worldData.PlayerSpawnRotation;
                break;
        }
    }
    private void Update()
    {
        if (TargetPlayer == null) return;

        if (TargetPlayer.position.y <= Limit_Y_Position)
        {
            _limitYPositionReached.OnNext((_respawnPosition, _respawnRotation));
        }
    }
}
