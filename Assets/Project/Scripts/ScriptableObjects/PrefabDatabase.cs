using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDatabase", menuName = "Scriptable Objects/PrefabDatabase")]
public class PrefabDatabase : ScriptableObject
{
    [SerializeField] private GameObject _playerPrefabForLobby;
    public GameObject PlayerPrefabForLobby => _playerPrefabForLobby;
    [SerializeField] private GameObject _playerPrefabForWorld;
    public GameObject PlayerPrefabForWorld => _playerPrefabForWorld;
    [SerializeField] private GameObject _syncedAvatarPrefab;
    public GameObject SyncedPlayerPrefab => _syncedAvatarPrefab;
    [SerializeField] private GameObject _syncPlayerRootPrefab;
    public GameObject SyncPlayerRootPrefab => _syncPlayerRootPrefab;
    [SerializeField] private GameObject _clientUIPrefab;
    public GameObject ClientUIPrefab => _clientUIPrefab;
    [SerializeField] private GameObject _worldListItemPrefab;
    public GameObject WorldListItemPrefab => _worldListItemPrefab;
    [SerializeField] private GameObject _avatarListItemPrefab;
    public GameObject AvatarListItemPrefab => _avatarListItemPrefab;
    [SerializeField] private GameObject _sessionItemPrefab;
    public GameObject SessionItemPrefab => _sessionItemPrefab;
    [SerializeField] private GameObject _sessionInfoItemPrefab;
    public GameObject SessionInfoItemPrefab => _sessionInfoItemPrefab;
    [SerializeField] private GameObject _networkRunnerPrefab;
    public GameObject NetworkRunnerPrefab => _networkRunnerPrefab;

}
