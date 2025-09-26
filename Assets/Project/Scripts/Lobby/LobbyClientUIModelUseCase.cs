using UnityEngine;
using VContainer;

public class LobbyClientUIModelUseCase : ClientUIModelUseCase
{
    public override WorldID TargetWorldID => WorldID.None;

    [Inject]
    public LobbyClientUIModelUseCase(WorldDatabase worldDatabase)
    {
        _worldDatabase = worldDatabase;
    }
}
