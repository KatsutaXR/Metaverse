using System;
using UniRx;
using UnityEngine;
using VContainer;

public class PlayerPresenter : IDisposable
{
    private PlayerView _playerView;
    private PlayerModel _playerModel;
    private RecorderController _recorderController;

    // Mediator経由のイベント
    private readonly Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    public IObservable<Unit> ToggleClientUIRequested => _toggleClientUIRequested;
    private readonly Subject<Vector3> _respawnButtonClicked = new Subject<Vector3>();
    public IObservable<Vector3> RespawnButtonClicked => _respawnButtonClicked;
    private CompositeDisposable _disposable;

    [Inject]
    public PlayerPresenter(PlayerModel playerModel, RecorderController recorderController)
    {
        _playerModel = playerModel;
        _recorderController = recorderController;
    }


    public void Initialize(PlayerView playerView)
    {
        _playerView = playerView;
        _disposable = new CompositeDisposable();

        _playerView
            .SetMicActiveRequested
            .Subscribe(_ => _recorderController.SetMicActive())
            .AddTo(_disposable);

        _playerView
            .ToggleClientUIRequested
            .Subscribe(_ =>
            {
                _playerModel.OnToggleClientUIRequested();
                _toggleClientUIRequested.OnNext(Unit.Default);
            })
            .AddTo(_disposable);

        _respawnButtonClicked
            .Subscribe(respawnPosition => _playerView.PlayerRespawn(respawnPosition))
            .AddTo(_disposable);
    }

    // Mediator経由でイベントを発火
    public void RequestRespawn(Vector3 respawnPosition)
    {
        _respawnButtonClicked.OnNext(respawnPosition);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

}
