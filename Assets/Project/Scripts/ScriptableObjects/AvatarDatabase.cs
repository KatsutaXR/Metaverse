using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarDatabase", menuName = "Scriptable Objects/AvatarDatabase")]
public class AvatarDatabase : ScriptableObject
{
    [SerializeField] private List<AvatarData> _avatars = new List<AvatarData>();
    public IReadOnlyList<AvatarData> Avatars => _avatars;
    public AvatarData GetAvatarById(AvatarID avatarID)
    {
        return _avatars.Find(avatar => avatar.AvatarID == avatarID);
    }
}
