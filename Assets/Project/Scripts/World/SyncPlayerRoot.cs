using Fusion;
using UnityEngine;

public class SyncPlayerRoot : NetworkBehaviour
{
    [SerializeField] SyncPlayerAvatar _syncPlayerAvatar;
    [SerializeField] SyncPlayerProfile _syncPlayerProfile;
    private Transform _targetOriginTransform;

    public void Initialize(PlayerReferences playerReferences)
    {
        _targetOriginTransform = playerReferences.Origin;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            transform.SetPositionAndRotation(_targetOriginTransform.position, _targetOriginTransform.rotation);
        }
    }

    public override void Render()
    {
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            transform.SetPositionAndRotation(_targetOriginTransform.position, _targetOriginTransform.rotation);
        }
    }

    /// <summary>
    /// AvatarUIからアバター変更のイベントが来たときに呼ばれる
    /// プレイヤー選択用Colliderのサイズは今は固定
    /// </summary>
    public void OnAvatarChangeRequested(AvatarID avatarID)
    {
        _syncPlayerAvatar.RpcSetupAvatar(avatarID);
        _syncPlayerAvatar.SetAvatarLayer();

        _syncPlayerProfile.UpdateProfilePosition(avatarID);
    }
}

