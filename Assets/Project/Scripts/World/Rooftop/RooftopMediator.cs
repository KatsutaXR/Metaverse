using VContainer;

public class RooftopMediator : WorldMediator
{
    [Inject]
    public RooftopMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter) :
    base(clientUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter) {}
}
