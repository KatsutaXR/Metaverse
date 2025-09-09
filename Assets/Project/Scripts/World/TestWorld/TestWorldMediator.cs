using UnityEngine;
using VContainer;
using UniRx;
using System;
using VContainer.Unity;

public class TestWorldMediator : IDisposable, IStartable
{
    private ClientUIPresenter _clientUIPresenter;
    private WorldUIPresenter _worldUIPresenter;
    private PlayerPresenter _playerPresenter;
    private CompositeDisposable _disposable;
    [Inject]
    public TestWorldMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();

        _clientUIPresenter
            .RespawnButtonClicked
            .Subscribe(respawnPosition => _playerPresenter.RequestRespawn(respawnPosition))
            .AddTo(_disposable);

        _clientUIPresenter
            .WorldButtonClicked
            .Subscribe(_ => _worldUIPresenter.RequestNavigateToWorldUI())
            .AddTo(_disposable);

        _worldUIPresenter
            .WorldListBackButtonClicked
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
