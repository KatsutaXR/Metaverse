using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// todo:MVPとして用意する
/// Model側で選択したアバターを記憶させたい
/// </summary>
public class AvatarView : MonoBehaviour
{
    [SerializeField] private Transform _targetHeadTransform;
    [SerializeField] private Transform _targetRightHandTransform;
    [SerializeField] private Transform _targetLeftHandTransform;
    [SerializeField] private Transform _avatarHeadTransform;
    [SerializeField] private Transform _avatarRightHandTransform;
    [SerializeField] private Transform _avatarLeftHandTransform;
    [SerializeField] private Animator _animator;

    [SerializeField] private InputActionReference _moveActionRef;
    [SerializeField] private InputActionReference _jumpActionRef; // todo:ジャンプを追加する

    // todo:avatarに応じて値を変えたい、位置は要検討
    [SerializeField] private Vector3 _fixLeftPosition;
    [SerializeField] private Quaternion _fixLeftRotation;
    [SerializeField] private Vector3 _fixRightPosition;
    [SerializeField] private Quaternion _fixRightRotation;

    private void OnEnable()
    {
        _moveActionRef.action.performed += OnMovePerformed;
        _moveActionRef.action.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        _moveActionRef.action.performed -= OnMovePerformed;
        _moveActionRef.action.canceled -= OnMoveCanceled;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _targetLeftHandTransform.position);
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _targetLeftHandTransform.rotation * _fixLeftRotation);

        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _targetRightHandTransform.position);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _targetRightHandTransform.rotation * _fixRightRotation);

        _animator.SetLookAtWeight(1.0f);
        _animator.SetLookAtPosition(_targetHeadTransform.position + _targetHeadTransform.forward * 10f);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        // todo:現在の状態(止まってる、走ってるなど)をEnumで定義して処理したい、inputの値に応じてアニメーションを変える
        _animator.SetBool("IsMove", true);
        _animator.SetFloat("InputX", input.x);
        _animator.SetFloat("InputY", input.y);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        // todo:現在の状態(止まってる、走ってるなど)をEnumで定義して処理したい
        _animator.SetBool("IsMove", false);
    }

    /// <summary>
    /// todo:アバターセットを変更する
    /// カメラのオフセットをアバターに応じて変える
    /// </summary>
    public void ChangeAvatar()
    {

    }
}
