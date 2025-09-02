using UnityEngine;

/// <summary>
/// 各プレイヤーが操作するアバターの参照を持つクラス
/// </summary>
public class AvatarData : MonoBehaviour
{
    [SerializeField] Transform _origin;
    public Transform Origin => _origin;
    [SerializeField] Transform _head;
    public Transform Head => _head;
    [SerializeField] Transform _body;
    public Transform Body => _body;
    [SerializeField] Transform _rightHand;
    public Transform RightHand => _rightHand;
    [SerializeField] Transform _leftHand;
    public Transform LeftHand => _leftHand;

}
