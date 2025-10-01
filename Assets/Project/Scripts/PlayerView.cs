using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private InputActionReference _toggleClientUIAction;
    [SerializeField] private InputActionReference _jumpAction; // todo:ジャンプを追加する
    [SerializeField] private InputActionReference _setMicActiveAction;
    [SerializeField] private GameObject _player;
    [SerializeField] private NearFarInteractor _rightNearFarInteractor;

    private readonly Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    public IObservable<Unit> ToggleClientUIRequested => _toggleClientUIRequested;
    private readonly Subject<Unit> _setMicActiveRequested = new Subject<Unit>();
    public IObservable<Unit> SetMicActiveRequested => _setMicActiveRequested;
    private readonly Subject<ProfileData> _showTargetProfileRequested = new Subject<ProfileData>();
    public IObservable<ProfileData> ShowTargetProfileRequested => _showTargetProfileRequested;


    private void OnEnable()
    {
        _toggleClientUIAction.action.Enable();
        _setMicActiveAction.action.Enable();

        _toggleClientUIAction.action.performed += OnToggleClientUIPerformed;
        _setMicActiveAction.action.performed += OnSetMicActivePerformed;

        _rightNearFarInteractor?.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        _toggleClientUIAction.action.performed -= OnToggleClientUIPerformed;
        _setMicActiveAction.action.performed -= OnSetMicActivePerformed;

        _rightNearFarInteractor?.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSetMicActivePerformed(InputAction.CallbackContext context)
    {
        _setMicActiveRequested.OnNext(Unit.Default);
    }

    private void OnToggleClientUIPerformed(InputAction.CallbackContext context)
    {
        _toggleClientUIRequested.OnNext(Unit.Default);
    }

    /// <summary>
    /// 選択したオブジェクトが他のプレイヤーだった場合の処理
    /// ワールドでのみ機能する
    /// </summary>
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 選択対象から SyncPlayerProfile を探す
        if (args.interactableObject.transform.TryGetComponent(out SyncPlayerProfile targetProfile))
        {
            ProfileData profileData = new ProfileData();
            profileData.Name = targetProfile.Name.ToString();
            profileData.SelfIntroduction = targetProfile.SelfIntroduction.ToString();

            _showTargetProfileRequested.OnNext(profileData);
        }
    }

    public void PlayerRespawn((Vector3, Quaternion) respawnData)
    {
        _player.transform.SetPositionAndRotation(respawnData.Item1, respawnData.Item2);
    }
}
