using Fusion;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SyncPlayerProfile : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnNameChanged))] public NetworkString<_32> Name { get; set; }
    [Networked] public NetworkString<_256> SelfIntroduction { get; set; }
    [Networked, OnChangedRender(nameof(OnPositionChanged))] public Vector3 LocalPosition { get; set; }
    [SerializeField] private TextMeshProUGUI _nameText;
    [Inject] private AvatarDatabase _avatarDatabase;
    [Inject] private AvatarStorage _avatarStorage;

    public void Initialize(ProfileStorage profileStorage)
    {
        var scope = FindAnyObjectByType<LifetimeScope>();
        scope.Container.Inject(this);

        // LayerをIgnoreLaycastにして自分のプロフィールを選択できないようにする
        gameObject.layer = 2;

        var profile = profileStorage.LoadProfile();
        Name = profile.Name;
        SelfIntroduction = profile.SelfIntroduction;

        var avatarID = _avatarStorage.Load();
        var avatarData = _avatarDatabase.GetAvatarById(avatarID);
        LocalPosition = avatarData.ProfileUIOffset;
    }

    public override void Spawned()
    {
        _nameText.text = Name.ToString();
        transform.localPosition = LocalPosition;
    }

    public void UpdateProfile(ProfileData profileData)
    {
        Name = profileData.Name;
        SelfIntroduction = profileData.SelfIntroduction;
    }

    public void UpdateProfilePosition(AvatarID avatarID)
    {
        var avatarData = _avatarDatabase.GetAvatarById(avatarID);
        LocalPosition = avatarData.ProfileUIOffset;
    }

    private void OnPositionChanged()
    {
        transform.localPosition = LocalPosition;
    }

    private void OnNameChanged()
    {
        _nameText.text = Name.ToString();
    }
}
