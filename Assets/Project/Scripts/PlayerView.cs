using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private InputActionReference _toggleClientUIAction;
    [SerializeField] private InputActionReference _jumpAction; // todo:ジャンプを追加する
    [SerializeField] private InputActionReference _setMicActiveAction;
    [SerializeField] private GameObject _player;

    private Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    private Subject<Unit> _setMicActiveRequested = new Subject<Unit>();

    public IObservable<Unit> ToggleClientUIRequested => _toggleClientUIRequested;
    public IObservable<Unit> SetMicActiveRequested => _setMicActiveRequested;

    private void OnEnable()
    {
        _toggleClientUIAction.action.Enable();
        _setMicActiveAction.action.Enable();

        _toggleClientUIAction.action.performed += OnToggleClientUIPerformed;
        _setMicActiveAction.action.performed += OnSetMicActivePerformed;
    }

    private void OnDisable()
    {
        _toggleClientUIAction.action.performed -= OnToggleClientUIPerformed;
        _setMicActiveAction.action.performed -= OnSetMicActivePerformed;
    }

    private void OnSetMicActivePerformed(InputAction.CallbackContext context)
    {
        _setMicActiveRequested.OnNext(Unit.Default);
    }

    private void OnToggleClientUIPerformed(InputAction.CallbackContext context)
    {
        _toggleClientUIRequested.OnNext(Unit.Default);
    }

    public void PlayerRespawn((Vector3, Quaternion) respawnData)
    {
        _player.transform.SetPositionAndRotation(respawnData.Item1, respawnData.Item2);
    }
}
