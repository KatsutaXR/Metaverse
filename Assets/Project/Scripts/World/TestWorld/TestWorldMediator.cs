using UnityEngine;
using VContainer;
using UniRx;

public class TestWorldMediator : WorldMediator
{
    [Inject]
    public TestWorldMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();
    }
}
