using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// 各プレイヤーが操作するアバターの参照を持つクラス
/// </summary>
public class PlayerReferences : MonoBehaviour
{
    [SerializeField] Transform _origin;
    public Transform Origin => _origin;
    [SerializeField] Transform _camera;
    public Transform Camera => _camera;
    [SerializeField] Transform _rightHand;
    public Transform RightHand => _rightHand;
    [SerializeField] Transform _leftHand;
    public Transform LeftHand => _leftHand;
    [SerializeField] XRBaseInteractor _rightNearFarInteractor;
    public XRBaseInteractor RightNearFarInteractor => _rightNearFarInteractor;
}
