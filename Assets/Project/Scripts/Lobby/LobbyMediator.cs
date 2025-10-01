using UnityEngine;
using VContainer;
using UniRx;
using System;
using VContainer.Unity;

public class LobbyMediator : IDisposable, IStartable
{
    private ClientUIPresenter _clientUIPresenter;
    private WorldUIPresenter _worldUIPresenter;
    private ProfileUIPresenter _profileUIPresenter;
    private PlayerPresenter _playerPresenter;
    private CompositeDisposable _disposable;
    [Inject]
    public LobbyMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();

        _clientUIPresenter
            .RespawnButtonClicked
            .Subscribe(respawnData => _playerPresenter.RequestRespawn(respawnData))
            .AddTo(_disposable);

        _profileUIPresenter
            .MyProfileBackButtonClicked
            .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
            .AddTo(_disposable);

        _clientUIPresenter
            .ProfileButtonClicked
            .Subscribe(_ => _profileUIPresenter.RequestNavigateToMyProfileUI())
            .AddTo(_disposable);

        // _worldUIPresenter
        //     .WorldListBackButtonClicked
        //     .Subscribe(_ => _clientUIPresenter.RequestBackToMainMenu())
        //     .AddTo(_disposable);

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
