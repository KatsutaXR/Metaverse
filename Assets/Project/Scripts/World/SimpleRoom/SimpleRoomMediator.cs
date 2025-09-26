using VContainer;
using UniRx;

public class SimpleRoomMediator : WorldMediator
{
    [Inject]
    public SimpleRoomMediator(ClientUIPresenter clientUIPresenter, WorldUIPresenter worldUIPresenter, PlayerPresenter playerPresenter)
    {
        _clientUIPresenter = clientUIPresenter;
        _worldUIPresenter = worldUIPresenter;
        _playerPresenter = playerPresenter;
        _disposable = new CompositeDisposable();
    }
}
