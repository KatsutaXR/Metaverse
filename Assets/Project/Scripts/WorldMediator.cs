using System;
using UniRx;
using VContainer.Unity;

public class WorldMediator : IDisposable, IStartable
{
    protected ClientUIPresenter _clientUIPresenter;
    protected WorldUIPresenter _worldUIPresenter;
    protected PlayerPresenter _playerPresenter;
    protected CompositeDisposable _disposable;

    public virtual void Start()
    {
        _clientUIPresenter
            .RespawnButtonClicked
            .Subscribe(respawnData => _playerPresenter.RequestRespawn(respawnData))
            .AddTo(_disposable);

        // _clientUIPresenter
        //     .WorldButtonClicked
        //     .Subscribe(_ => _worldUIPresenter.RequestNavigateToWorldUI())
        //     .AddTo(_disposable);

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
}
