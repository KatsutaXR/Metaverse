using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;
public class SessionInfoUIController : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionStartTime;
    [SerializeField] private TextMeshProUGUI _numberOfPeople;
    [SerializeField] private NetworkObject _sessionInfoContent;
    [SerializeField] private PrefabDatabase _prefabDatabase;
    [Networked] public NetworkString<_16> SessionStartTime { get; set; }
    [Networked, OnChangedRender(nameof(OnNumberOfPeopleChanged))] public NetworkString<_16> NumberOfPeople { get; set; }
    [Networked, Capacity(128)] public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayerNames => default;
    [Inject] private ProfileStorage _profileStorage;
    private int _lastChildCount = 0;

    public override void Spawned()
    {
        var scope = FindAnyObjectByType<LifetimeScope>();
        scope.Container.Inject(this);

        if (Runner.IsSharedModeMasterClient)
        {
            DateTime dt = DateTime.Now;
            string currentTime = dt.ToString("HH:mm:ss");
            SessionStartTime = currentTime;
            NumberOfPeople = $"{Runner.SessionInfo.PlayerCount} / {Runner.SessionInfo.MaxPlayers}";
        }

        _sessionStartTime.text = SessionStartTime.ToString();
        _numberOfPeople.text = NumberOfPeople.ToString();

        // todo:sessionInfoPrefabを生成し、各項目を設定して同期する
        var obj = Runner.Spawn(_prefabDatabase.SessionInfoItemPrefab);
        var playerName = _profileStorage.LoadName();
        obj.GetComponent<SessionInfoItemController>().SetupSessionInfoItem(SessionInfoType.Join, playerName, _sessionInfoContent.Id);

        if (Object.HasStateAuthority) PlayerNames.Add(Runner.LocalPlayer, playerName);
        else RpcSetPlayerName(Runner.LocalPlayer, playerName);
    }

    public override void Render()
    {
        if (_sessionInfoContent.transform.childCount != _lastChildCount)
        {
            _lastChildCount = _sessionInfoContent.transform.childCount;
            SortItemsByTime();
        }
    }

    private void SortItemsByTime()
    {
        var items = new List<SessionInfoItemController>();

        foreach (Transform child in _sessionInfoContent.transform)
        {
            if (child.TryGetComponent(out SessionInfoItemController item)) items.Add(item);
        }

        // Timestamp昇順でソート
        items = items.OrderBy(i => i.SimulationTime).ToList();

        // SetSiblingIndexで反映
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.SetSiblingIndex(i);
        }
    }

    public void UpdateNumberOfPeople()
    {
        Debug.Log("UpdateNumberOfPeople");
        NumberOfPeople = $"{Runner.SessionInfo.PlayerCount} / {Runner.SessionInfo.MaxPlayers}";
    }

    private void OnNumberOfPeopleChanged()
    {
        _numberOfPeople.text = NumberOfPeople.ToString();
    }

    public void SpawnLeaveSessionInfoItem(PlayerRef targetPlayer)
    {
        Debug.Log("SpawnLeaveSerssionInfoItem");
        var obj = Runner.Spawn(_prefabDatabase.SessionInfoItemPrefab);
        var playerName = PlayerNames[targetPlayer];
        PlayerNames.Remove(targetPlayer);
        obj.GetComponent<SessionInfoItemController>().SetupSessionInfoItem(SessionInfoType.Leave, playerName.ToString(), _sessionInfoContent.Id);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcSetPlayerName(PlayerRef player, string playerName)
    {
        PlayerNames.Add(player, playerName);
    }
}
