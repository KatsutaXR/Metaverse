using System;
using Fusion;
using UniRx;
using UnityEngine;
using VContainer;

public class WorldUIModel : IDisposable
{
    private PrefabDatabase _prefabDatabase;
    public GameObject SessionItemPrefab => _prefabDatabase.SessionItemPrefab;
    public WorldID TargetWorldID { get; set; }
    public SessionInfo TargetSessionInfo { get; set; }
    private CompositeDisposable _disposable;

    [Inject]
    public WorldUIModel(PrefabDatabase prefabDatabase)
    {
        _disposable = new CompositeDisposable();

        _prefabDatabase = prefabDatabase;
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
