using System;
using UniRx;
using VContainer;

public class AvatarUIPresenter : IDisposable
{
    private AvatarUIView _avatarUIView;
    private AvatarUIModel _avatarUIModel;
    private AvatarStorage _avatarStorage;
    private PlayerXRUtility _playerXRUtility;
    private SyncPlayerRoot _syncPlayerRoot;
    private AvatarView _avatarView;

    private readonly Subject<Unit> _avatarListBackButtonClicked = new Subject<Unit>();
    public IObservable<Unit> AvatarListBackButtonClicked => _avatarListBackButtonClicked;
    private readonly Subject<Unit> _navigateToAvatarListUIRequested = new Subject<Unit>();

    private CompositeDisposable _disposable;

    [Inject]
    public AvatarUIPresenter(AvatarUIModel avatarUIModel, AvatarStorage avatarStorage, PlayerXRUtility playerXRUtility)
    {
        _avatarUIModel = avatarUIModel;
        _avatarStorage = avatarStorage;
        _playerXRUtility = playerXRUtility;
    }

    /// <summary>
    /// syncPlayerRootの参照の有無でロビーとワールドとで処理を分ける
    /// 複雑化する場合は変更する
    /// </summary>
    public void Initialize(AvatarUIView avatarUIView, SyncPlayerRoot syncPlayerRoot = null, AvatarView avatarView = null)
    {
        _avatarUIView = avatarUIView;
        _syncPlayerRoot = syncPlayerRoot;
        _avatarView = avatarView;
        _disposable = new CompositeDisposable();

        _avatarUIView
            .AvatarListSaveButtonClicked
            .Subscribe(_ =>
            {
                var currentAvatarID = _avatarStorage.Load();
                var selectedAvatarID = _avatarUIModel.SelectedAvatarID;

                if (selectedAvatarID == AvatarID.None || selectedAvatarID == currentAvatarID) return;

                _avatarStorage.Save(selectedAvatarID);

                // ワールドでの処理
                if (syncPlayerRoot != null) _syncPlayerRoot.OnAvatarChangeRequested(selectedAvatarID);
                // ロビーでの処理
                else _avatarView.SetupAvatar(selectedAvatarID);

                _playerXRUtility.Recenter();
            })
            .AddTo(_disposable);

        _avatarUIView
            .AvatarListItemClicked
            .Subscribe(avatarID => _avatarUIModel.SelectedAvatarID = avatarID)
            .AddTo(_disposable);

        // 以下Mediator経由のイベント

        _avatarUIView
            .AvatarListBackButtonClicked
            .Subscribe(_ =>
            {
                _avatarUIModel.SelectedAvatarID = AvatarID.None;
                _avatarListBackButtonClicked.OnNext(Unit.Default);
            })
            .AddTo(_disposable);

        _navigateToAvatarListUIRequested
            .Subscribe(_ => _avatarUIView.GoToAvatarList())
            .AddTo(_disposable);
    }

    public void RequestNavigateToAvatarListUI()
    {
        _navigateToAvatarListUIRequested.OnNext(Unit.Default);
    }
    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
