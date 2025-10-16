using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;

public class SessionInfoItemController : NetworkBehaviour
{
    private const string Type_Join = "[Join]";
    private const string Type_Leave = "[Leave]";
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _infoType;
    [SerializeField] private TextMeshProUGUI _playerName;
    [Networked] public NetworkString<_16> Time { get; set; }
    [Networked] public SessionInfoType InfoType { get; set; }
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public NetworkId ParentId { get; set; }
    [Networked] public double SimulationTime { get; set; }
    private bool _parentSet = false;

    /// <summary>
    /// 後から参加したプレイヤーに同期させる関数
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSyncSessionInfoItem([RpcTarget] PlayerRef targetPlayer)
    {
        _time.text = Time.ToString();

        switch (InfoType)
        {
            case SessionInfoType.Join:
                _infoType.color = Color.green;
                _infoType.text = Type_Join;
                break;
            case SessionInfoType.Leave:
                _infoType.color = Color.red;
                _infoType.text = Type_Leave;
                break;
            default:
                break;
        }

        _playerName.text = PlayerName.ToString();

        WaitForParentAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    /// <summary>
    /// 参加時にセッション情報を設定する関数
    /// Rpcで他のクライアントにも同期させる
    /// </summary>
    public void SetupSessionInfoItem(SessionInfoType type, string playerName, NetworkId parentId)
    {
        DateTime dt = DateTime.Now;
        string currentTime = dt.ToString("HH:mm:ss");
        Time = currentTime;

        InfoType = type;
        PlayerName = playerName;
        ParentId = parentId;
        SimulationTime = Runner.SimulationTime;

        RpcSetupSessionInfoItem(currentTime, type, playerName, parentId);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcSetupSessionInfoItem(string currentTime, SessionInfoType type, string playerName, NetworkId parentId)
    {
        _time.text = currentTime;

        switch (type)
        {
            case SessionInfoType.Join:
                _infoType.color = Color.green;
                _infoType.text = Type_Join;
                break;
            case SessionInfoType.Leave:
                _infoType.color = Color.red;
                _infoType.text = Type_Leave;
                break;
            default:
                break;
        }

        _playerName.text = playerName;

        WaitForParentAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    /// <summary>
    /// 特定のオブジェクトの子供として設定するための関数
    /// 親をネットワークオブジェクトとしているため、親の同期が完了してオブジェクトが見つかるのを待つ
    /// </summary>
    private async UniTask WaitForParentAsync(CancellationToken token)
    {
        while (!_parentSet && !token.IsCancellationRequested)
        {
            if (Runner.TryFindObject(ParentId, out var parentObj))
            {
                transform.SetParent(parentObj.transform, false);
                _parentSet = true;
            }
            await UniTask.Yield();
        }
    }
}