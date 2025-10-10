using VContainer;

public class TestWorldMediator : WorldMediator
{
    [Inject]
    public TestWorldMediator(ClientUIPresenter clientUIPresenter, ProfileUIPresenter profileUIPresenter, AvatarUIPresenter avatarUIPresenter, PlayerPresenter playerPresenter) :
    base(clientUIPresenter, profileUIPresenter, avatarUIPresenter, playerPresenter) {}
}
