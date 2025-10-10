using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

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
    [SerializeField] private Vector3 _fixLeftPosition;
    [SerializeField] private Quaternion _fixLeftRotation;
    [SerializeField] private Vector3 _fixRightPosition;
    [SerializeField] private Quaternion _fixRightRotation;
    [Inject] private AvatarDatabase _avatarDatabase;
    [Inject] private AvatarStorage _avatarStorage;
    private AvatarID _lastAvatarID = AvatarID.None;
    private GameObject _currentAvatar;

    public void Initialize()
    {
        var scope = FindAnyObjectByType<LifetimeScope>();
        scope.Container.Inject(this);

        var avatarID = _avatarStorage.Load();
        SetupAvatar(avatarID);
    }

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
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _targetLeftHandTransform.position + (_targetLeftHandTransform.rotation * _fixLeftPosition));
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _targetLeftHandTransform.rotation * _fixLeftRotation);

        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _targetRightHandTransform.position + (_targetRightHandTransform.rotation * _fixLeftPosition));
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _targetRightHandTransform.rotation * _fixRightRotation);

        _animator.SetLookAtWeight(1.0f);
        _animator.SetLookAtPosition(_targetHeadTransform.position + _targetHeadTransform.forward * 10f);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        _animator.SetBool("IsMove", true);
        _animator.SetFloat("InputX", input.x);
        _animator.SetFloat("InputY", input.y);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _animator.SetBool("IsMove", false);
    }

    /// <summary>
    /// アバターセットを変更する
    /// 自分のアバター表示については要検討
    /// </summary>
    public void SetupAvatar(AvatarID avatarID)
    {
        if (avatarID == _lastAvatarID) return;
        var avatarData = _avatarDatabase.GetAvatarById(avatarID);
        _lastAvatarID = avatarID;

        if (_currentAvatar != null) Destroy(_currentAvatar);
        var obj = Instantiate(avatarData.AvatarPrefab);
        _currentAvatar = obj;
        obj.transform.SetParent(transform);
        obj.transform.SetPositionAndRotation(transform.position, transform.rotation);

        _animator.avatar = avatarData.AnimatorAvatar;
        // 要検討
        // _avatarHand =
        // _fix...    

        SetAvatarLayer();
    }

    /// <summary>
    /// 自身のアバター表示を設定する
    /// mixamoのモデルが一つのメッシュで構成されているものが多いためすべて隠しておく
    /// </summary>
    private void SetAvatarLayer()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = 7; // InvisibleAvatar
        }
        // _avatarHand.layer = 6; // VisibleAvatar
    }
}
