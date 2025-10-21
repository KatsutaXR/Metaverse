using System;
using UniRx;

/// <summary>
/// ネットワークオブジェクトを制御するクラスの参照を中継するクラス
/// Spawn時にPublishで自分自身をイベントに載せて送る
/// 各Initializeより先にSpawnされることもあるためReplaySubjectで直近のイベントを後からでも購読できるようにする
/// </summary>
public class NetworkSpawnEventStream
{
    private readonly ReplaySubject<SyncVideo> _onSyncVideoSpawned = new ReplaySubject<SyncVideo>(1);
    public IObservable<SyncVideo> OnSyncVideoSpawned => _onSyncVideoSpawned;

    public void PublishSyncVideoSpawned(SyncVideo video) => _onSyncVideoSpawned.OnNext(video);
}
