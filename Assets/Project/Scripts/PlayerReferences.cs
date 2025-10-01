using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// 各プレイヤーが操作するアバターの参照を持つクラス
/// </summary>
public class PlayerReferences : MonoBehaviour
{
    [SerializeField] private Transform _origin;
    public Transform Origin => _origin;
    [SerializeField] private Transform _camera;
    public Transform Camera => _camera;
    [SerializeField] private Transform _rightHand;
    public Transform RightHand => _rightHand;
    [SerializeField] private Transform _leftHand;
    public Transform LeftHand => _leftHand;
    [SerializeField] private XRBaseInteractor _rightNearFarInteractor;
    public XRBaseInteractor RightNearFarInteractor => _rightNearFarInteractor;
    [SerializeField] private ProfileFilter _rightNearFarProfileFilter;
    public ProfileFilter RightNearFarProfileFilter => _rightNearFarProfileFilter;
}
