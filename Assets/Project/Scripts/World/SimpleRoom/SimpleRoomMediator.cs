using VContainer;

public class SimpleRoomMediator : WorldMediator
{
    [Inject]
    public SimpleRoomMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter) :
    base(clientUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter) {}
}
