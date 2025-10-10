using UnityEngine;

/// <summary>
/// アバターの設定を保持する
/// 今はIDだけだが、項目が増えればAvatarID→AvatarDataとする
/// </summary>
public class AvatarStorage
{
    private string _path = System.IO.Path.Combine(Application.persistentDataPath, "avatar.json");

    public void Save(AvatarID avatarID)
    {
        var json = JsonUtility.ToJson(avatarID, true);
        System.IO.File.WriteAllText(_path, json);
    }

    public AvatarID Load()
    {
        if (!System.IO.File.Exists(_path)) return new AvatarID();
        var json = System.IO.File.ReadAllText(_path);
        return JsonUtility.FromJson<AvatarID>(json);
    }
}
