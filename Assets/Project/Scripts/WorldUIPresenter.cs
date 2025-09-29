using System;
using Fusion;
using UniRx;
using UnityEngine;
using VContainer;

public class WorldUIPresenter : IDisposable
{
    private WorldUIView _worldUIView;
    private WorldUIModel _worldUIModel;
    private NetworkController _networkController;
    // private readonly Subject<Unit> _worldListBackButtonClicked = new Subject<Unit>();
    // public IObservable<Unit> WorldListBackButtonClicked => _worldListBackButtonClicked;
    // Mediator経由のイベント
    // private readonly Subject<Unit> _navigateToWorldUIRequested = new Subject<Unit>();
    private CompositeDisposable _disposable;

    [Inject]
    public WorldUIPresenter(WorldUIModel worldUIModel, NetworkController networkController)
    {
        _worldUIModel = worldUIModel;
        _networkController = networkController;
    }

    public void Initialize(WorldUIView worldUIView)
    {
        _worldUIView = worldUIView;
        _disposable = new CompositeDisposable();

        _worldUIView
            .WorldListItemClicked
            .Subscribe(worldID =>
            {
                _worldUIModel.TargetWorldID = worldID;
                var sessionInfos = _networkController.FindTargetSessions(worldID);
                _worldUIView.UpdateSessionItems(sessionInfos, _worldUIModel.SessionItemPrefab);
            })
            .AddTo(_disposable);

        _worldUIView
            .SessionItemClicked
            .Subscribe(sessionInfo => _worldUIModel.TargetSessionInfo = sessionInfo)
            .AddTo(_disposable);

        _worldUIView
            .UpdateSessionItemsRequested
            .Subscribe(_ =>
            {
                var sessionInfos = _networkController.FindTargetSessions(_worldUIModel.TargetWorldID);
                _worldUIView.UpdateSessionItems(sessionInfos, _worldUIModel.SessionItemPrefab);
            })
            .AddTo(_disposable);

        _worldUIView
            .CreateSessionRequested
            .Subscribe(customSessionInfo =>_networkController.CreateSessionAsync(_worldUIModel.PrepareCreateSessionInfo(customSessionInfo)))
            .AddTo(_disposable);

        _worldUIView
            .JoinSessionRequested
            .Subscribe(customSessionInfo => _networkController.JoinSessionAsync(_worldUIModel.PrepareJoinSessionInfo(customSessionInfo)))
            .AddTo(_disposable);

        // 以下Mediator経由のイベント

        // _worldUIView
        //     .WorldListBackButtonClicked
        //     .Subscribe(_ => _worldListBackButtonClicked.OnNext(Unit.Default))
        //     .AddTo(_disposable);

        // _navigateToWorldUIRequested
        //     .Subscribe(_ => _worldUIView.GoToWorldUI())
        //     .AddTo(_disposable);
    }

    // public void RequestNavigateToWorldUI()
    // {
    //     _navigateToWorldUIRequested.OnNext(Unit.Default);
    // }
    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
