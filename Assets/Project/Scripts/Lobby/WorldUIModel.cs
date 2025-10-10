using System;
using Fusion;
using UniRx;
using UnityEngine;
using VContainer;

public class WorldUIModel : IDisposable
{
    private PrefabDatabase _prefabDatabase;
    private WorldDatabase _worldDatabase;
    public GameObject SessionItemPrefab => _prefabDatabase.SessionItemPrefab;
    public WorldID TargetWorldID { get; set; }
    public SessionInfo TargetSessionInfo { get; set; }
    private CompositeDisposable _disposable;

    [Inject]
    public WorldUIModel(PrefabDatabase prefabDatabase, WorldDatabase worldDatabase)
    {
        _disposable = new CompositeDisposable();

        _prefabDatabase = prefabDatabase;
        _worldDatabase = worldDatabase;
    }

    public CustomSessionInfo PrepareCreateSessionInfo(CustomSessionInfo customSessionInfo)
    {
        customSessionInfo.WorldID = TargetWorldID;
        customSessionInfo.MaxPlayers = _worldDatabase.GetWorldById(TargetWorldID).MaxPlayers;
        return customSessionInfo;
    }

    public CustomSessionInfo PrepareJoinSessionInfo(CustomSessionInfo customSessionInfo)
    {
        customSessionInfo.WorldID = TargetWorldID;
        customSessionInfo.SessionName = TargetSessionInfo.Name;
        return customSessionInfo;
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
