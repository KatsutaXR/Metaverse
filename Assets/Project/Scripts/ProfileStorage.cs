using UnityEngine;

public class ProfileStorage
{
    private string _path = System.IO.Path.Combine(Application.persistentDataPath, "profile.json");

    public void Save(ProfileData profileData)
    {
        var json = JsonUtility.ToJson(profileData, true);
        System.IO.File.WriteAllText(_path, json);
    }

    public ProfileData LoadProfile()
    {
        if (!System.IO.File.Exists(_path)) return new ProfileData();
        var json = System.IO.File.ReadAllText(_path);
        return JsonUtility.FromJson<ProfileData>(json);
    }

    public string LoadName()
    {
        if (!System.IO.File.Exists(_path)) return "";
        var json = System.IO.File.ReadAllText(_path);
        return JsonUtility.FromJson<ProfileData>(json).Name;
    }

    public string LoadIntro()
    {
        if (!System.IO.File.Exists(_path)) return "";
        var json = System.IO.File.ReadAllText(_path);
        return JsonUtility.FromJson<ProfileData>(json).SelfIntroduction;
    }
}
