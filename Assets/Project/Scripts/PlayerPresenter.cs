using System;
using UniRx;
using UnityEngine;
using VContainer;

public class PlayerPresenter : IDisposable
{
    private PlayerView _playerView;
    private PlayerModel _playerModel;
    private RecorderController _recorderController;
    private RespawnAreaController _respawnAreaController;

    // Mediator経由のイベント
    private readonly Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    public IObservable<Unit> ToggleClientUIRequested => _toggleClientUIRequested;
    private readonly Subject<ProfileData> _showTargetProfileRequested = new Subject<ProfileData>();
    public IObservable<ProfileData> ShowTargetProfileRequested => _showTargetProfileRequested;
    private readonly Subject<(Vector3, Quaternion)> _respawnButtonClicked = new Subject<(Vector3, Quaternion)>();
    private CompositeDisposable _disposable;

    [Inject]
    public PlayerPresenter(PlayerModel playerModel, RecorderController recorderController, RespawnAreaController respawnAreaController)
    {
        _playerModel = playerModel;
        _recorderController = recorderController;
        _respawnAreaController = respawnAreaController;
    }


    public void Initialize(PlayerView playerView)
    {
        _playerView = playerView;
        _disposable = new CompositeDisposable();

        _playerView
            .SetMicActiveRequested
            .Subscribe(_ => _recorderController.SetMicActive())
            .AddTo(_disposable);


        _respawnAreaController
            .LimitYPositionReached
            .Subscribe(respawnData => _playerView.PlayerRespawn(respawnData))
            .AddTo(_disposable);

        // 以下Mediator経由

        _playerView
            .ToggleClientUIRequested
            .Subscribe(_ =>
            {
                _playerModel.OnToggleClientUIRequested();
                _toggleClientUIRequested.OnNext(Unit.Default);
            })
            .AddTo(_disposable);

        _playerView
            .ShowTargetProfileRequested
            .Subscribe(profiledata => _showTargetProfileRequested.OnNext(profiledata))
            .AddTo(_disposable);

        _respawnButtonClicked
            .Subscribe(respawnData => _playerView.PlayerRespawn(respawnData))
            .AddTo(_disposable);

    }

    // Mediator経由でイベントを発火
    public void RequestRespawn((Vector3, Quaternion) respawnData)
    {
        _respawnButtonClicked.OnNext(respawnData);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

}
