using UnityEngine;
using VContainer;

public class LobbyObjectFactory
{
    private PrefabDatabase _prefabDatabase;

    [Inject]
    public LobbyObjectFactory(PrefabDatabase prefabDatabase)
    {
        _prefabDatabase = prefabDatabase;
    }

    public GameObject CreatePlayer()
    {
        return Object.Instantiate(_prefabDatabase.PlayerPrefabForLobby);
    }

    /// <summary>
    /// 各クライアントが操作するUIを作成する
    /// </summary>
    /// <returns></returns>
    public GameObject CreateClientUI()
    {
        GameObject clientUI = Object.Instantiate(_prefabDatabase.ClientUIPrefab);
        return clientUI;
    }
}
