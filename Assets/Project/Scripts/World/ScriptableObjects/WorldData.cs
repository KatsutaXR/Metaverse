using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "Scriptable Objects/WorldData")]
/// <summary>
/// 各Worldの情報を持つScriptableObject
/// 要検討
/// </summary>
public class WorldData : ScriptableObject
{
    [SerializeField] private WorldID _worldID;
    [SerializeField] private string _worldName;
    [SerializeField] private string _worldDescription;
    [SerializeField] private int _maxPlayers;
    [SerializeField] private Sprite _worldImage;

    public WorldID WorldID => _worldID;
    public string WorldName => _worldName;
    public string WorldDescription => _worldDescription; 
    public int MaxPlayers => _maxPlayers;
    public Sprite WorldImage => _worldImage;
}
