using System;
using UniRx;
using VContainer;

public class ProfileUIPresenter : IDisposable
{
    private ProfileUIModel _profileUIModel;
    private ProfileUIView _profileUIView;
    private ProfileStorage _profileStorage;
    private CompositeDisposable _disposable;

    private readonly Subject<Unit> _myProfileBackButtonClicked = new Subject<Unit>();
    public IObservable<Unit> MyProfileBackButtonClicked => _myProfileBackButtonClicked;
    private readonly Subject<Unit> _navigateToMyProfileUIRequested = new Subject<Unit>();
    private readonly Subject<ProfileData> _navigateToTargetProfileUIRequested = new Subject<ProfileData>();

    [Inject]
    public ProfileUIPresenter(ProfileUIModel profileUIModel, ProfileStorage profileStorage)
    {
        _profileUIModel = profileUIModel;
        _profileStorage = profileStorage;
    }

    /// <summary>
    /// ロビーとワールドでそれぞれ処理が変わる
    /// 差分が大きくなれば実装を変更する
    /// </summary>
    public void Initialize(ProfileUIView profileUIView, SyncPlayerProfile syncPlayerProfile = null)
    {
        _profileUIView = profileUIView;
        _disposable = new CompositeDisposable();

        _profileUIView
            .MyProfileSaveButtonClicked
            .Subscribe(profileData =>
            {
                _profileStorage.Save(profileData);
                // ワールドなら同期用Profileを更新する
                syncPlayerProfile?.UpdateProfile(profileData);
            })
            .AddTo(_disposable);


        // 以下Mediator経由のイベント

        _profileUIView
            .MyProfileBackButtonClicked
            .Subscribe(_ => _myProfileBackButtonClicked.OnNext(Unit.Default))
            .AddTo(_disposable);

        _navigateToMyProfileUIRequested
            .Subscribe(_ => _profileUIView.GoToMyProfileUI(_profileStorage.LoadProfile()))
            .AddTo(_disposable);

        _navigateToTargetProfileUIRequested
            .Subscribe(profileData => _profileUIView.GoToTargetProfileUI(profileData))
            .AddTo(_disposable);
    }

    public void RequestNavigateToMyProfileUI()
    {
        _navigateToMyProfileUIRequested.OnNext(Unit.Default);
    }

    public void RequestNavigateToTargetProfileUI(ProfileData profileData)
    {
        _navigateToTargetProfileUIRequested.OnNext(profileData);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
