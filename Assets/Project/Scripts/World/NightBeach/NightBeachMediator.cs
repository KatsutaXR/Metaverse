using VContainer;
public class NightBeachMediator : WorldMediator
{
    [Inject]
    public NightBeachMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter) :
    base(clientUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter) {}
}
