using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PenController : NetworkBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private NetworkPrefabRef _strokePrefab;
    [SerializeField] private InputActionReference _drawActionRef;
    [SerializeField] private Button _clearButton;
    [SerializeField] private float _segmentLength = 0.000001f;
    [SerializeField] private XRBaseInteractor _xrBaseinteractor;

    // 線を一括削除できるように、各ペンで書いた線に対して固有のタグを持たせる
    [Networked] public string UniquePenTag { get; set; }
    private StrokeController _currentStroke;
    private Vector3 _lastPos;

    public void Initialize(XRBaseInteractor xRBaseInteractor)
    {
        _clearButton.onClick.AddListener(DeleteAllStroke);
        _xrBaseinteractor = xRBaseInteractor;

        // マスタークライアントが各ペン固有のタグを設定する
        if (Runner.IsSharedModeMasterClient)
        {
            Object.ReleaseStateAuthority();
            UniquePenTag = "Pen_" + Guid.NewGuid().ToString();
        }
    }

    private void OnEnable()
    {
        _drawActionRef.action.Enable();
        _drawActionRef.action.canceled += OnDrawCanceled;
    }

    public override void Render()
    {
        if (_xrBaseinteractor == null) return;
        if (_xrBaseinteractor.firstInteractableSelected == null) return;

        
        var grabbed = _xrBaseinteractor.firstInteractableSelected?.transform.gameObject;
        if (grabbed != gameObject) return;

        Debug.Log($"grabbed = {grabbed}");

        if (!HasStateAuthority)
        {
            Debug.Log("!HasStateAuthority");
            Object.RequestStateAuthority();
        }
        
        if (_drawActionRef.action.IsPressed())
        {
            DrawStroke();
        }
    }

    private void DrawStroke()
    {
        if (_currentStroke == null)
        {
            _currentStroke = Runner.Spawn(_strokePrefab, _tip.position, Quaternion.identity, Runner.LocalPlayer).GetComponent<StrokeController>();
            _currentStroke.PenTag = UniquePenTag;
            _lastPos = _tip.position;
        }

        if (_currentStroke != null)
        {
            float dist = Vector3.Distance(_tip.position, _lastPos);
            
            if (dist >= _segmentLength)
            {
                _currentStroke.AddPoint(_tip.position, _segmentLength);
                _lastPos = _tip.position;
            }
        }
    }

    private void OnDrawCanceled(InputAction.CallbackContext ctx)
    {
        var grabbed = _xrBaseinteractor.firstInteractableSelected?.transform.gameObject;
        if (grabbed != gameObject) return;

        if (_currentStroke != null)
        {
            _currentStroke = null; // 完了、保持したまま残す
        }
    }

    private void DeleteAllStroke()
    {
        foreach (var stroke in GameObject.FindObjectsByType<StrokeController>(FindObjectsSortMode.None))
        {
            if (stroke.PenTag != UniquePenTag) continue;
            if (stroke.TryGetComponent<NetworkObject>(out var netObj))
            {
                Runner.Despawn(netObj); // 全クライアントに反映
            }
        }
    }
}
