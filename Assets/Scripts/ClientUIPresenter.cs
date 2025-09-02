
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
    private AvatarData _avatarData;
    // Mediator経由のイベント
    private readonly Subject<Vector3> _respawnButtonClicked = new Subject<Vector3>();
    public IObservable<Vector3> RespawnButtonClicked => _respawnButtonClicked;
    private readonly Subject<Unit> _worldButtonClicked = new Subject<Unit>();
    public IObservable<Unit> WorldButtonClicked => _worldButtonClicked;
    private readonly Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    private readonly Subject<Unit> _backToMainMenuRequested = new Subject<Unit>();
    private CompositeDisposable _disposable;

    [Inject]
    public ClientUIPresenter(ClientUIModel clientUIModel, NetworkController networkController)
    {
        _clientUIModel = clientUIModel;
        _networkController = networkController;
    }

    public void Initialize(ClientUIView clientUIView, AvatarData avatarData)
    {
        _clientUIView = clientUIView;
        _avatarData = avatarData;
        _disposable = new CompositeDisposable();

        // todo:UIの位置をクライアントに合わせる

        // ロビーに戻るボタン押下時
        _clientUIView
            .LobbyButtonClicked
            .Subscribe(_ => _networkController.JoinLobbyAsync().Forget())
            .AddTo(_disposable);


        // 以下Mediator経由のイベント

        // リスポーンボタン押下時
        _clientUIView
            .RespawnButtonClicked
            .Subscribe(_ => _respawnButtonClicked.OnNext(_clientUIModel.Respawn()))
            .AddTo(_disposable);
        
        // ワールドボタン押下時
        _clientUIView
            .WorldButtonClicked
            .Subscribe(_ => _worldButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        // ClientUIを切り替えるボタン(コントローラー)押下時
        _toggleClientUIRequested
            .Subscribe(_ =>
            {
                (Vector3, Quaternion) clientUIInfo = _clientUIModel.CorrectionClientUITransform();
                _clientUIView.ToggleClientUI(clientUIInfo.Item1, clientUIInfo.Item2);
            })
            .AddTo(_disposable);

        // WorldUIのバックボタン押下時
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
