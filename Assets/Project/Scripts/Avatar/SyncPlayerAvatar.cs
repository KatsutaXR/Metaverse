using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class SyncPlayerAvatar : NetworkBehaviour
{
    private bool _isMoveActionRegisterd = false;
    private Transform _targetHeadTransform;
    private Transform _targetRightHandTransform;
    private Transform _targetLeftHandTransform;
    private Vector3 _interpHeadPosition;
    private Quaternion _interpHeadRotation;
    private Vector3 _interpHeadForward;
    private Vector3 _interpRightHandPosition;
    private Quaternion _interpRightHandRotation;
    private Vector3 _interpLeftHandPosition;
    private Quaternion _interpLeftHandRotation;
    [Networked] public Vector3 TargetHeadPosition { get; set; }
    [Networked] public Quaternion TargetHeadRotation { get; set; }
    [Networked] public Vector3 TargetHeadForward { get; set; }
    [Networked] public Vector3 TargetRightHandPosition { get; set; }
    [Networked] public Quaternion TargetRightHandRotation { get; set; }
    [Networked] public Vector3 TargetLeftHandPosition { get; set; }
    [Networked] public Quaternion TargetLeftHandRotation { get; set; }

    [SerializeField] private GameObject _avatarHand;
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

    // Initializeはスポーンを行うクライアント下でのみ呼ばれる
    public void Initialize(PlayerReferences playerReferences)
    {
        var scope = FindAnyObjectByType<LifetimeScope>();
        scope.Container.Inject(this);

        _targetHeadTransform = playerReferences.Camera;
        _targetRightHandTransform = playerReferences.RightHand;
        _targetLeftHandTransform = playerReferences.LeftHand;

        var avatarID = _avatarStorage.Load();
        RpcSetupAvatar(avatarID);
        SetAvatarLayer();
    }

    public void SetAvatarLayer()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = 7; // InvisibleAvatar
        }
        // _avatarHand.layer = 6; // VisibleAvatar
    }

    private void OnEnable()
    {
        if (Object != null && Object.InputAuthority == Runner.LocalPlayer && !_isMoveActionRegisterd)
        {
            _moveActionRef.action.performed += OnMovePerformed;
            _moveActionRef.action.canceled += OnMoveCanceled;
            _isMoveActionRegisterd = true;
        }
    }

    private void OnDisable()
    {
        if (Object != null && Object.InputAuthority == Runner.LocalPlayer && _isMoveActionRegisterd)
        {
            _moveActionRef.action.performed -= OnMovePerformed;
            _moveActionRef.action.canceled -= OnMoveCanceled;
            _isMoveActionRegisterd = false;
        }
    }

    public override void Spawned()
    {
        // InjectのタイミングがRpcSetupAvatarより後になる可能性あり
        var scope = FindAnyObjectByType<LifetimeScope>();
        scope.Container.Inject(this);

        if (Object.InputAuthority == Runner.LocalPlayer && !_isMoveActionRegisterd)
        {
            _moveActionRef.action.performed += OnMovePerformed;
            _moveActionRef.action.canceled += OnMoveCanceled;
            _isMoveActionRegisterd = true;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.InputAuthority == Runner.LocalPlayer && _isMoveActionRegisterd)
        {
            _moveActionRef.action.performed -= OnMovePerformed;
            _moveActionRef.action.canceled -= OnMoveCanceled;
            _isMoveActionRegisterd = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            TargetHeadPosition = _targetHeadTransform.position;
            TargetHeadRotation = _targetHeadTransform.rotation;
            TargetHeadForward = _targetHeadTransform.forward;
            TargetRightHandPosition = _targetRightHandTransform.position;
            TargetRightHandRotation = _targetRightHandTransform.rotation;
            TargetLeftHandPosition = _targetLeftHandTransform.position;
            TargetLeftHandRotation = _targetLeftHandTransform.rotation;
        }
    }

    public override void Render()
    {   
        if (Object.InputAuthority != Runner.LocalPlayer)
        {
            var interpolator = new NetworkBehaviourBufferInterpolator(this);

            _interpHeadPosition = interpolator.Vector3(nameof(TargetHeadPosition));
            _interpHeadRotation = interpolator.Quaternion(nameof(TargetHeadRotation));
            _interpHeadForward = interpolator.Vector3(nameof(TargetHeadForward));
            _interpRightHandPosition = interpolator.Vector3(nameof(TargetRightHandPosition));
            _interpRightHandRotation = interpolator.Quaternion(nameof(TargetRightHandRotation));
            _interpLeftHandPosition = interpolator.Vector3(nameof(TargetLeftHandPosition));
            _interpLeftHandRotation = interpolator.Quaternion(nameof(TargetLeftHandRotation));
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetupAvatar(AvatarID avatarID)
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
    }

    public void SyncAvatar(PlayerRef targetPlayer)
    {
        var avatarID = _avatarStorage.Load();
        RpcSyncAvatar(targetPlayer, avatarID);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSyncAvatar([RpcTarget] PlayerRef targetPlayer, AvatarID avatarID)
    {
        if (Object.HasStateAuthority) return;
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
    }

    /// <summary>
    /// 各ローカル内でアニメーターの同期を行わせる
    /// </summary>
    /// <param name="layerIndex"></param>
    private void OnAnimatorIK(int layerIndex)
    {
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            _animator.SetLookAtWeight(1.0f);
            _animator.SetLookAtPosition(_targetHeadTransform.position + _targetHeadTransform.forward * 10f);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _targetRightHandTransform.position + (_targetRightHandTransform.rotation * _fixRightPosition));
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _targetRightHandTransform.rotation * _fixRightRotation);

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _targetLeftHandTransform.position + (_targetLeftHandTransform.rotation * _fixLeftPosition));
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _targetLeftHandTransform.rotation * _fixLeftRotation);
        }
        else
        {
            _animator.SetLookAtWeight(1.0f);
            _animator.SetLookAtPosition(_interpHeadPosition + _interpHeadForward * 10f);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _interpRightHandPosition + (_interpRightHandRotation * _fixRightPosition));
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _interpRightHandRotation * _fixRightRotation);

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _interpLeftHandPosition + (_interpLeftHandRotation * _fixLeftPosition));
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _interpLeftHandRotation * _fixLeftRotation);
        }
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
}
