using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using VContainer;

public class VideoPresenter : IDisposable
{
    private VideoView _videoView;
    private VideoModel _videoModel;
    private NetworkSpawnEventStream _networkSpawnEventStream;
    private SyncVideo _syncVideo;
    private CompositeDisposable _disposable;

    [Inject]
    public VideoPresenter(VideoModel videoModel, NetworkSpawnEventStream networkSpawnEventStream)
    {
        _videoModel = videoModel;
        _networkSpawnEventStream = networkSpawnEventStream;
    }

    public void Initialize(VideoView videoView)
    {
        _videoView = videoView;
        _disposable = new CompositeDisposable();

        _networkSpawnEventStream
            .OnSyncVideoSpawned
            .Subscribe(syncVideo => OnSyncVideoSpawned(syncVideo))
            .AddTo(_disposable);
    }

    private void OnSyncVideoSpawned(SyncVideo syncVideo)
    {
        _syncVideo = syncVideo;

        _videoView
            .RnadomButtonClicked
            .Subscribe(_ => _syncVideo.RpcSetRandom())
            .AddTo(_disposable);

        _videoView
            .PreviousButtonClicked
            .Subscribe(_ => _syncVideo.RpcSetVideoIndex(_videoModel.GetPreviousVideoIndex(_syncVideo.CurrentVideoIndex), _videoModel.Playlist.Count))
            .AddTo(_disposable);

        _videoView
            .PlayPauseButtonClicked
            .Subscribe(_ => _syncVideo.RpcSetPlayState())
            .AddTo(_disposable);

        _videoView
            .NextButtonClicked
            .Subscribe(_ => _syncVideo.RpcSetVideoIndex(_videoModel.GetNextVideoIndex(_syncVideo.CurrentVideoIndex), _videoModel.Playlist.Count))
            .AddTo(_disposable);

        _videoView
            .PlaylistItemClicked
            .Subscribe(index => _syncVideo.RpcSetVideoIndex(index, _videoModel.Playlist.Count))
            .AddTo(_disposable);

        _videoView
            .LoopButtonClicked
            .Subscribe(_ => _syncVideo.RpcSetLoop())
            .AddTo(_disposable);

        _videoView
            .SeekBarValueUpdated
            .Subscribe(value => _syncVideo.RpcChangeSeekBarValue(value))
            .AddTo(_disposable);

        _videoView
            .VideoEnded
            .Subscribe(_ =>
            {
                if (!_syncVideo.CheckState()) return;
                int nextIndex = _videoModel.DecideNextVideoByState(_syncVideo.CurrentVideoIndex, _syncVideo.IsRandom, _syncVideo.IsLoop, _syncVideo.VideoHistory.ToList());
                _syncVideo.RpcSetVideoIndex(nextIndex, _videoModel.Playlist.Count);
            })
            .AddTo(_disposable);


        _syncVideo
            .RandomSettingChanged
            .Subscribe(random => _videoView.SetRandom(random))
            .AddTo(_disposable);

        _syncVideo
            .VideoIndexChanged
            .Subscribe(index => _videoView.SetVideoContent(_videoModel.Playlist[index].Url, _videoModel.Playlist[index].Name))
            .AddTo(_disposable);

        _syncVideo
            .PlayStateChanged
            .Subscribe(playState => _videoView.SetPlayState(playState))
            .AddTo(_disposable);

        _syncVideo
            .LoopSettingChanged
            .Subscribe(loop => _videoView.SetLoop(loop))
            .AddTo(_disposable);

        _syncVideo
            .SeekBarValueChanged
            .Subscribe(value => _videoView.SetVideoTime(value).Forget())
            .AddTo(_disposable);

        _syncVideo
            .GetCurrentSeekBarValueRequested
            .Subscribe(_ => _syncVideo.SetCurrentSeekBarValue(_videoView.GetCurrentSeekBarValue()))
            .AddTo(_disposable);
    }
    
    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
