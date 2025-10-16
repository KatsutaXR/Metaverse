using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PenController : NetworkBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private InputActionReference _drawActionRef;
    [SerializeField] private Button _clearButton;
    [SerializeField] private float _segmentLength = 0.01f;
    [SerializeField] private StrokeController _strokeController;
    [SerializeField] private NetworkObjectGrabController _networkObjectGrabController;

    private bool _isDrawing;
    private Vector3 _lastPos;

    public override void Spawned()
    {
        _clearButton.onClick.AddListener(RequestDeleteStrokes);
        _isDrawing = false;

        _drawActionRef.action.Enable();
        _drawActionRef.action.canceled += OnDrawCanceled;
    }

    public override void Render()
    {

        if (!Object.HasStateAuthority || !_networkObjectGrabController.IsGrabbingThisObj) return;

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
