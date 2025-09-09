using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private InputActionReference _toggleClientUIAction;
    [SerializeField] private InputActionReference _jumpAction; // todo:ジャンプを追加する
    [SerializeField] private InputActionReference _xButtonAction;
    [SerializeField] private GameObject _player;

    private Subject<Unit> _toggleClientUIRequested = new Subject<Unit>();
    private Subject<Unit> _onXButtonPressed = new Subject<Unit>();

    public IObservable<Unit> ToggleClientUIRequested => _toggleClientUIRequested;
    public IObservable<Unit> OnXButtonPressed => _onXButtonPressed;

    private void OnEnable()
    {
        _toggleClientUIAction.action.Enable();
        _toggleClientUIAction.action.performed += OnToggleClientUIPerformed;
        _xButtonAction.action.performed += OnXPerformed;
    }

    private void OnDisable()
    {
        _toggleClientUIAction.action.performed -= OnToggleClientUIPerformed;
        _xButtonAction.action.performed -= OnXPerformed;
    }

    private void OnXPerformed(InputAction.CallbackContext context)
    {
        _onXButtonPressed.OnNext(Unit.Default);
    }

    private void OnToggleClientUIPerformed(InputAction.CallbackContext context)
    {
        _toggleClientUIRequested.OnNext(Unit.Default);
    }

    public void PlayerRespawn(Vector3 respawnPosition)
    {
        Debug.Log($"PlayerRespawn:Position = {respawnPosition}");
        _player.transform.position = respawnPosition;
    }
}
