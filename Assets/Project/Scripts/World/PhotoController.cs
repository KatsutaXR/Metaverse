using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoController : NetworkBehaviour
{
    private const int PHOTO_MAX_COUNT = 15;
    [SerializeField] private InputActionReference _photoActionRef;
    [SerializeField] private NetworkPrefabRef _photoPrefab;
    [SerializeField] private Camera _captureCamera;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private NetworkObjectGrabController _networkObjectGrabController;
    [Networked, Capacity(16)] public NetworkLinkedList<NetworkId> PhotoDisplayIDs => default;

    public override void Spawned()
    {
        _photoActionRef.action.Enable();
        _photoActionRef.action.performed += OnTakePhotoPerformed;
    }

    private void OnDisable()
    {
        _photoActionRef.action.performed -= OnTakePhotoPerformed;
    }
    
    // 退出時にaction.perfomedから外さないとずっと購読されてしまう
    private void OnTakePhotoPerformed(InputAction.CallbackContext context)
    {
        if (!Object.HasStateAuthority || !_networkObjectGrabController.IsGrabbingThisObj) return;

        int targetTick = Runner.Tick + 5;


        // todo:ポジションを設定する
        var photo = Runner.Spawn(_photoPrefab, position: transform.position + Vector3.down * 0.25f, rotation: transform.rotation);
        CheckPhotoCounts(photo.Id);

        RpcOnTakePhotoPerformed(targetTick, photo.Id);
    }

    private void CheckPhotoCounts(NetworkId targetId)
    {
        PhotoDisplayIDs.Add(targetId);

        if (PhotoDisplayIDs.Count <= PHOTO_MAX_COUNT) return;

        NetworkObject obj = Runner.FindObject(PhotoDisplayIDs[0]);
        if (obj != null)
        {
            obj.GetComponent<PhotoDisplayController>().RpcDespawnRequest();
        }

        var fistId = PhotoDisplayIDs[0];
        PhotoDisplayIDs.Remove(fistId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcOnTakePhotoPerformed(int targetTick, NetworkId photoId)
    {
        _audioSource.Play();
        WaitForTickAndTakePhoto(targetTick, photoId).Forget();
    }

    private async UniTask WaitForTickAndTakePhoto(int targetTick, NetworkId photoId)
    {
        // Tickが指定値に到達するまで待機
        await UniTask.WaitUntil(() => Runner.Tick >= targetTick);

        // --- PhotoDisplayがスポーンされるまで待機 ---
        NetworkObject photoObj = null;
        await UniTask.WaitUntil(() =>
        {
            photoObj = Runner.FindObject(photoId);
            return photoObj != null;
        });

        // --- フレーム終端まで待ってレンダリング安定化 ---
        await UniTask.WaitForEndOfFrame();

        // --- 撮影処理 ---
        var textures = CaptureToTexture();

        if (photoObj.TryGetComponent(out PhotoDisplayController display))
        {
            display.SetupTexture(textures.Item1, textures.Item2);
        }
        else
        {
            Debug.LogWarning($"PhotoDisplay not found on object {photoId}");
        }

    }
    
    private (Texture2D, Texture2D) CaptureToTexture()
    {
        var rt = _captureCamera.targetTexture;

        var localTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, true);
        var syncTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false, false);

        // sRGBへ変換したRenderTextureを別途作成
        var tempRt = RenderTexture.GetTemporary(rt.width, rt.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        // BlitでsRGB変換
        Graphics.Blit(rt, tempRt);

        RenderTexture.active = rt;
        localTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        localTexture.Apply();

        RenderTexture.active = tempRt;
        syncTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        syncTexture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRt);
        
        return (localTexture, syncTexture);
    }
}
