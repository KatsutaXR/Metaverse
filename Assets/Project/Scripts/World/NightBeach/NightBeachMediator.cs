using VContainer;
using UniRx;

public class NightBeachMediator : WorldMediator
{
    [Inject]
    public NightBeachMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();
    }
}
