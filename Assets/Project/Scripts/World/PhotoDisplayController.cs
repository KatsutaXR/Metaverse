using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PhotoDisplayController : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    private Texture2D _syncTexture;
    private ReliableKey _key;

    public void SetupTexture(Texture2D localTexture, Texture2D syncTexture)
    {
        _syncTexture = syncTexture;
        ApplyTexture(localTexture);
    }

    public void ApplyTexture(Texture2D texture)
    {
        var block = new MaterialPropertyBlock();
        block.SetTexture("_BaseMap", texture);
        _renderer.SetPropertyBlock(block);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcDespawnRequest()
    {
        Runner.Despawn(Object);
    }

    /// <summary>
    /// 後から参加したクライアントに写真を同期する関数
    /// </summary>
    public void SyncPhoto(PlayerRef targetPlayer)
    {
        if (Object.StateAuthority == PlayerRef.None)
        {
            if (!Runner.IsSharedModeMasterClient) return;
        }
        else
        {
            if (!Object.HasStateAuthority) return;
        }

        SendPhotoData(targetPlayer);
    }

    private void SendPhotoData(PlayerRef targetPlayer)
    {
        // 内部的にsRGBに変換してから圧縮
        byte[] imageBytes = _syncTexture.EncodeToJPG(75);

        // 今はNetworkIDをKeyに保持させ、一致するオブジェクトにデータを処理させる
        _key = ReliableKey.FromInts((int)Object.Id.Raw, 0, 0, 0);
        Runner.SendReliableDataToPlayer(targetPlayer, _key, imageBytes);
    }

    // 受信データを適用する
    public void ApplyTextureFromBytes(byte[] bytes)
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        if (!texture.LoadImage(bytes))
        {
            Debug.LogError("[PhotoSyncStream] Failed to LoadImage");
            return;
        }

        ApplyTexture(texture);
    }


}
