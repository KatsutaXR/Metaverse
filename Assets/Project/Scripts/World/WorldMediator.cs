using System;
using UniRx;
using VContainer.Unity;

public abstract class WorldMediator : IDisposable, IStartable
{
    protected ClientUIPresenter _clientUIPresenter;
    protected ProfileUIPresenter _profileUIPresenter;
    protected AvatarUIPresenter _avatarUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected CompositeDisposable _disposable;

    protected WorldMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _avatarUIPresenter = avatarUIPresenter;
        _playerPresenter = playerPresenter;

        _disposable = new CompositeDisposable();
    }

    public virtual void Start()
    {
        _clientUIPresenter
            .RespawnButtonClicked
            .Subscribe(respawnData => _playerPresenter.RequestRespawn(respawnData))
            .AddTo(_disposable);

        _clientUIPresenter
            .ProfileButtonClicked
            .Subscribe(_ => _profileUIPresenter.RequestNavigateToMyProfileUI())
            .AddTo(_disposable);

        _clientUIPresenter
            .AvatarButtonClicked
            .Subscribe(_ => _avatarUIPresenter.RequestNavigateToAvatarListUI())
            .AddTo(_disposable);
        
        _avatarUIPresenter
            .AvatarListBackButtonClicked
            .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
            .AddTo(_disposable);

        _profileUIPresenter
            .MyProfileBackButtonClicked
            .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
            .AddTo(_disposable);

        _playerPresenter
            .ToggleClientUIRequested
            .Subscribe(_ => _clientUIPresenter.RequestToggleClientUI())
            .AddTo(_disposable);

        _playerPresenter
            .ShowTargetProfileRequested
            .Subscribe(profileData => _profileUIPresenter.RequestNavigateToTargetProfileUI(profileData))
            .AddTo(_disposable);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
