using System;
using Fusion;
using UniRx;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SyncVideo : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnRandomSettingChanged))] public bool IsRandom { get; set; }
    [Networked, OnChangedRender(nameof(OnVideoIndexChanged))] public int CurrentVideoIndex { get; set; }
    [Networked, OnChangedRender(nameof(OnPlayStateChanged))] public bool IsPlaying { get; set; }
    [Networked, OnChangedRender(nameof(OnLoopSettingChanged))] public bool IsLoop { get; set; }
    [Networked, OnChangedRender(nameof(OnSeekBarValueChanged))] public float ChangedSeekBarValue { get; set; }
    // 現在のシークバーの値を定期的に更新しておく
    // Sliderを操作して値が変更した時とプロパティを共有すると更新タイミング的に問題があるため分けた
    [Networked] public float CurrentSeekBarValue { get; set; }
    // プレイリストの上限を30とする
    [Networked, Capacity(30)] public NetworkLinkedList<int> VideoHistory => default;

    // Spawned()のタイミングではまだ購読が完了していないことがあるため、
    // 購読前に呼ばれたイベントも購読できるようにReplaySubjectを使用する
    private readonly ReplaySubject<bool> _randomSettingChanged = new ReplaySubject<bool>(1);
    public IObservable<bool> RandomSettingChanged => _randomSettingChanged;
    private readonly ReplaySubject<int> _videoIndexChanged = new ReplaySubject<int>(1);
    public IObservable<int> VideoIndexChanged => _videoIndexChanged;
    private readonly ReplaySubject<bool> _playStateChanged = new ReplaySubject<bool>(1);
    public IObservable<bool> PlayStateChanged => _playStateChanged;
    private readonly ReplaySubject<bool> _loopSettingChanged = new ReplaySubject<bool>(1);
    public IObservable<bool> LoopSettingChanged => _loopSettingChanged;
    private readonly ReplaySubject<float> _seekBarValueChanged = new ReplaySubject<float>(1);
    public IObservable<float> SeekBarValueChanged => _seekBarValueChanged;
    private readonly ReplaySubject<Unit> _getCurrentSeekBarValueRequested = new ReplaySubject<Unit>(1);
    public IObservable<Unit> GetCurrentSeekBarValueRequested => _getCurrentSeekBarValueRequested;

    [Inject] private NetworkSpawnEventStream _networkSpawnEventStream;

    private float _totalTime = 0;
    private float _span = 0.5f;

    public override void Spawned()
    {
        var scope = FindAnyObjectByType<WorldLifetimeScope>();
        scope.Container.Inject(this);
        // このクラスの参照を必要とするクラスに向けて自分自身を送る
        _networkSpawnEventStream.PublishSyncVideoSpawned(this);

        if (Object.HasStateAuthority)
        {
            IsRandom = false;
            CurrentVideoIndex = 0;
            IsPlaying = false;
            IsLoop = false;
            ChangedSeekBarValue = 0;
            CurrentSeekBarValue = 0;
            VideoHistory.Add(0);
        }

        Initialize().Forget();
    }

    public override void Render()
    {
        if (!Object.HasStateAuthority) return;

        _totalTime += Time.deltaTime;

        if (_totalTime > _span)
        {
            _totalTime = 0;
            _getCurrentSeekBarValueRequested.OnNext(Unit.Default);
        }
    }

    private async UniTask Initialize()
    {
        _randomSettingChanged.OnNext(IsRandom);
        _videoIndexChanged.OnNext(CurrentVideoIndex);
        _playStateChanged.OnNext(IsPlaying);
        _loopSettingChanged.OnNext(IsLoop);
        // CurrentSeekBarValueで同期をとる
        await UniTask.WaitForSeconds(1);
        _seekBarValueChanged.OnNext(CurrentSeekBarValue);
    }

    private void OnRandomSettingChanged()
    {
        _randomSettingChanged.OnNext(IsRandom);
    }

    private void OnVideoIndexChanged()
    {
        _videoIndexChanged.OnNext(CurrentVideoIndex);
    }

    private void OnPlayStateChanged()
    {
        _playStateChanged.OnNext(IsPlaying);
    }

    private void OnLoopSettingChanged()
    {
        _loopSettingChanged.OnNext(IsLoop);
    }

    private void OnSeekBarValueChanged()
    {
        _seekBarValueChanged.OnNext(ChangedSeekBarValue);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetRandom()
    {
        IsRandom = !IsRandom;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetVideoIndex(int index, int playlistLength)
    {
        if (CurrentVideoIndex != index) CurrentVideoIndex = index;
        IsPlaying = true;

        // 再生履歴を更新する
        if (!VideoHistory.Contains(index))
        {
            VideoHistory.Add(index);
            // 動画を一巡したら履歴リセット
            if (VideoHistory.Count >= playlistLength) VideoHistory.Clear();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetPlayState()
    {
        IsPlaying = !IsPlaying;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcSetLoop()
    {
        IsLoop = !IsLoop;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcChangeSeekBarValue(float value)
    {
        ChangedSeekBarValue = value;
    }

    // 状態権限保持者で定期的に呼ばれる
    public void SetCurrentSeekBarValue(float value)
    {
        CurrentSeekBarValue = value;
    }

    public bool CheckState()
    {
        return Object.HasStateAuthority;
    }
}
