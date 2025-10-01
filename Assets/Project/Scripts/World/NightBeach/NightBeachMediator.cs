using VContainer;
using UniRx;

public class NightBeachMediator : WorldMediator
{
    [Inject]
    public NightBeachMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();
    }
}
