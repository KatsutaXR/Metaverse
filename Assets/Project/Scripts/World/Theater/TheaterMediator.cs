using VContainer;

public class TheaterMediator : WorldMediator
{
    [Inject]
    public TheaterMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter) :
    base(clientUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter) {}
}
