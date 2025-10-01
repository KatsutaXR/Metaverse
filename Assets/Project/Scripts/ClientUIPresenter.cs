
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

public class ClientUIPresenter : IDisposable
{
    // [Inject]で依存注入
    private NetworkController _networkController;

    // Initializeで参照取得
    private ClientUIView _clientUIView;
    private ClientUIModel _clientUIModel;
    // Mediator経由のイベント
    private readonly Subject<(Vector3, Quaternion)> _respawnButtonClicked = new Subject<(Vector3, Quaternion)>();
    public IObservable<(Vector3, Quaternion)> RespawnButtonClicked => _respawnButtonClicked;
    private readonly Subject<Unit> _profileButtonClicked = new Subject<Unit>();
    public IObservable<Unit> ProfileButtonClicked => _profileButtonClicked;
    private readonly Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    private readonly Subject<Unit> _backToMainMenuRequested = new Subject<Unit>();
    private CompositeDisposable _disposable;

    [Inject]
    public ClientUIPresenter(ClientUIModel clientUIModel, NetworkController networkController)
    {
        _clientUIModel = clientUIModel;
        _networkController = networkController;
    }

    public void Initialize(ClientUIView clientUIView)
    {
        _clientUIView = clientUIView;
        _disposable = new CompositeDisposable();

        // ロビーに戻るボタン押下時
        _clientUIView
            .LobbyButtonClicked
            .Subscribe(_ => _networkController.JoinLobbyAsync().Forget())
            .AddTo(_disposable);

        _clientUIView
            .UpdateTransformRequested
            .Subscribe(_ =>
            {
                var clientUITransform = _clientUIModel.CorrectionClientUITransform();
                _clientUIView.UpdateClientUiTransform(clientUITransform.Item1, clientUITransform.Item2);
            });

        // 以下Mediator経由のイベント

        // リスポーンボタン押下時
        _clientUIView
            .RespawnButtonClicked
            .Subscribe(_ => _respawnButtonClicked.OnNext(_clientUIModel.Respawn()))
            .AddTo(_disposable);
        
        // プロフィールボタン押下時
        _clientUIView
            .ProfileButtonClicked
            .Subscribe(_ => _profileButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        // ClientUIを切り替えるボタン(コントローラー)押下時
        _toggleClientUIRequested
            .Subscribe(_ =>
            {
                var clientUITransform = _clientUIModel.CorrectionClientUITransform();
                _clientUIView.ToggleClientUI(clientUITransform.Item1, clientUITransform.Item2);
            })
            .AddTo(_disposable);

        // MainMenuに戻るBackButton押下時
        _backToMainMenuRequested
            .Subscribe(_ => _clientUIView.BackToMainMenu())
            .AddTo(_disposable);

    }

    public void RequestToggleClientUI()
    {
        _toggleClientUIRequested.OnNext(Unit.Default);
    }

    public void RequestBackToMainMenu()
    {
        _backToMainMenuRequested.OnNext(Unit.Default);
    }

    // 紐づいているLifetimeScopeの破棄と同時に呼ばれる
    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
