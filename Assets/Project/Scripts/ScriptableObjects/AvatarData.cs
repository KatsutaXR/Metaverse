using UnityEngine;

[CreateAssetMenu(fileName = "AvatarData", menuName = "Scriptable Objects/AvatarData")]
/// <summary>
/// 各Avatarの情報を持つScriptableObject
/// </summary>
public class AvatarData : ScriptableObject
{
    [SerializeField] private AvatarID _avatarID;
    [SerializeField] private string _avatarName;
    [SerializeField] private GameObject _avatarPrefab;
    [SerializeField] private Sprite _avatarImage;
    [SerializeField] private Avatar _animatorAvatar;
    [SerializeField] private float _cameraYOffset;
    [SerializeField] private Vector3 _profileUIOffset;

    public AvatarID AvatarID => _avatarID;
    public string AvatarName => _avatarName;
    public GameObject AvatarPrefab => _avatarPrefab;
    public Sprite AvatarImage => _avatarImage;
    public Avatar AnimatorAvatar => _animatorAvatar;
    public float CameraYOffset => _cameraYOffset;
    public Vector3 ProfileUIOffset => _profileUIOffset;
}
