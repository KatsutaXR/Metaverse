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
    [SerializeField] private XRBaseInteractor _xrBaseinteractor;
    [SerializeField] private StrokeManager _strokeManager;

    private bool _isDrawing;
    private Vector3 _lastPos;

    public void Initialize(XRBaseInteractor xRBaseInteractor)
    {
        _clearButton.onClick.AddListener(RequestDeleteStrokes);
        _xrBaseinteractor = xRBaseInteractor;
        _isDrawing = false;

        if (Runner.IsSharedModeMasterClient) Object.ReleaseStateAuthority();
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
        if (_isDrawing)
        {
            float dist = Vector3.Distance(_tip.position, _lastPos);

            if (dist >= _segmentLength)
            {
                _strokeManager.AddPoint(false, _tip.position);
                _lastPos = _tip.position;
            }
        }
        else
        {
            _strokeManager.AddPoint(true, _tip.position);
            _lastPos = _tip.position;
            _isDrawing = true;
        }

    }

    private void OnDrawCanceled(InputAction.CallbackContext ctx)
    {
        var grabbed = _xrBaseinteractor.firstInteractableSelected?.transform.gameObject;
        if (grabbed != gameObject) return;

        _isDrawing = false;
    }

    private void RequestDeleteStrokes()
    {
        Debug.Log("RequestDeleteStrokes");
        _isDrawing = false;
        _strokeManager.DeleteStrokes();
    }
}
