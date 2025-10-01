using VContainer;
using UniRx;

public class RooftopMediator : WorldMediator
{
    [Inject]
    public RooftopMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, ProfileUIPresenter profileUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _profileUIPresenter = profileUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();
    }
}
