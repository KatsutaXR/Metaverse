using System;
using UniRx;
using VContainer.Unity;

public class WorldMediator : IDisposable, IStartable
{
    protected ClientUIPresenter _clientUIPresenter;
    protected WorldUIPresenter _worldUIPresenter;
    protected ProfileUIPresenter _profileUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected CompositeDisposable _disposable;

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

        // _worldUIPresenter
        //     .WorldListBackButtonClicked
        //     .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
        //     .AddTo(_disposable);

    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}
