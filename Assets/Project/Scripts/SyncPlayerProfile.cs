using Fusion;
using TMPro;
using UnityEngine;

public class SyncPlayerProfile : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnNameChanged))] public NetworkString<_32> Name { get; set; }
    [Networked] public NetworkString<_256> SelfIntroduction { get; set; }
    [SerializeField] private TextMeshProUGUI _nameText;

    public void Initialize(ProfileStorage profileStorage)
    {
        var profile = profileStorage.LoadProfile();
        Name = profile.Name;
        SelfIntroduction = profile.SelfIntroduction;
    }

    public override void Spawned()
    {
        _nameText.text = Name.ToString();
    }

    public void UpdateProfile(ProfileData profileData)
    {
        Name = profileData.Name;
        SelfIntroduction = profileData.SelfIntroduction;
    }

    private void OnNameChanged()
    {
        _nameText.text = (string)Name;
    }
}
