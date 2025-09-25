using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PenController : NetworkBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private InputActionReference _drawActionRef;
    [SerializeField] private Button _clearButton;
    [SerializeField] private float _segmentLength = 0.01f;
    [SerializeField] private StrokeController _strokeController;

    private bool _isDrawing;
    private Vector3 _lastPos;
    private XRBaseInteractor _xrBaseinteractor;

    public override void Spawned()
    {
        _clearButton.onClick.AddListener(RequestDeleteStrokes);
        _isDrawing = false;

        if (Runner.IsSharedModeMasterClient) Object.ReleaseStateAuthority();

        _drawActionRef.action.Enable();
        _drawActionRef.action.canceled += OnDrawCanceled;
    }

    public override void Render()
    {
        if (_xrBaseinteractor == null)
        {
            _xrBaseinteractor = FindFirstObjectByType<PlayerReferences>()?.RightNearFarInteractor;
            if (_xrBaseinteractor == null) return;
        }

        if (_xrBaseinteractor.firstInteractableSelected == null) return;
        
        var grabbed = _xrBaseinteractor.firstInteractableSelected?.transform.gameObject;
        if (grabbed != gameObject) return;

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
        if (_isDrawing)
        {
            float dist = Vector3.Distance(_tip.position, _lastPos);

            if (dist >= _segmentLength)
            {
                _strokeController.AddPoint(false, _tip.position);
                _lastPos = _tip.position;
            }
        }
        else
        {
            _strokeController.AddPoint(true, _tip.position);
            _lastPos = _tip.position;
            _isDrawing = true;
        }

    }

    private void OnDrawCanceled(InputAction.CallbackContext ctx)
    {
        if (_isDrawing) _isDrawing = false;
    }

    private void RequestDeleteStrokes()
    {
        Debug.Log("RequestDeleteStrokes");
        _isDrawing = false;
        _strokeController.DeleteStrokes();
    }
}
