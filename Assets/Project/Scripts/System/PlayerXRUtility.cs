using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using VContainer;

public class PlayerXRUtility
{
    private AvatarStorage _avatarStorage;
    private AvatarDatabase _avatarDatabase;
    [Inject]
    public PlayerXRUtility(AvatarStorage avatarStorage, AvatarDatabase avatarDatabase)
    {
        _avatarStorage = avatarStorage;
        _avatarDatabase = avatarDatabase;
    }

    /// <summary>
    /// 現在のHMDの位置でカメラの位置をリセットする
    /// TryRecenter()が動作しないため、オフセットオブジェクトのTransformを調整して基本的に操作する
    /// </summary>
    public void Recenter()
    {
        XROrigin xrOrigin = GameObject.FindAnyObjectByType<XROrigin>(FindObjectsInactive.Include);
        Transform origin = xrOrigin.transform;
        var currentAvatarID = _avatarStorage.Load();
        var avatarData = _avatarDatabase.GetAvatarById(currentAvatarID);

        var camera = xrOrigin.Camera.transform;

        // 回転調整
        var camForward = camera.forward;
        camForward.y = 0f;
        camForward.Normalize();

        var originForward = origin.forward;
        originForward.y = 0f;
        originForward.Normalize();

        var deltaRotation = Quaternion.FromToRotation(originForward, camForward);
        origin.rotation *= deltaRotation;
        xrOrigin.CameraFloorOffsetObject.transform.localRotation *= Quaternion.Inverse(deltaRotation);

        // 位置調整
        Vector3 corectedCameraPos = origin.position + Vector3.up * avatarData.CameraYOffset;
        Vector3 positionOffset = camera.position - corectedCameraPos;
        xrOrigin.CameraFloorOffsetObject.transform.position -= positionOffset;
    }

    /// <summary>
    /// シーンをまたぐとすぐにHMDのトラッキングが開始しないため開始を待ってからRecenterする
    /// todo:HMDのトラッキング開始まで待つ処理に変える
    /// </summary>
    public async UniTask WaitTrackingStartAndRecenter(float waitTime = 0.1f)
    {
        await UniTask.WaitForSeconds(waitTime);
        Recenter();
    }
}
