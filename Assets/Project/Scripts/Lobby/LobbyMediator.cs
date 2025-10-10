using UnityEngine;
using VContainer;
using UniRx;
using System;
using VContainer.Unity;

public class LobbyMediator : IDisposable, IStartable
{
    private ClientUIPresenter _clientUIPresenter;
    private ProfileUIPresenter _profileUIPresenter;
    private AvatarUIPresenter _avatarUIPresenter;
    private PlayerPresenter _playerPresenter;
    private CompositeDisposable _disposable;
    [Inject]
    public LobbyMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _avatarUIPresenter = avatarUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();

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

        _profileUIPresenter
            .MyProfileBackButtonClicked
            .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
            .AddTo(_disposable);

        _avatarUIPresenter
            .AvatarListBackButtonClicked
            .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
            .AddTo(_disposable);

        _playerPresenter
            .ToggleClientUIRequested
            .Subscribe(_ => _clientUIPresenter.RequestToggleClientUI())
            .AddTo(_disposable);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void Start()
    {
        Debug.Log("LobbyMediator");
    }
}
