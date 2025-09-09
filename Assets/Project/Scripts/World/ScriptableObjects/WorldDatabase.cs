using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDatabase", menuName = "Scriptable Objects/WorldDatabase")]
public class WorldDatabase : ScriptableObject
{
    [SerializeField] private List<WorldData> _worlds = new List<WorldData>();
    public IReadOnlyList<WorldData> Worlds => _worlds;
    public WorldData GetWorldById(WorldID worldID)
    {
        return _worlds.Find(world => world.WorldID == worldID);
    }
}
