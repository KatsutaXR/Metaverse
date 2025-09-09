using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenController : NetworkBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private NetworkPrefabRef _strokePrefab;
    [SerializeField] private InputActionReference _drawActionRef;
    [SerializeField] private float _segmentLength = 0.1f;

    private GameObject _currentStroke;
    private Vector3 _lastSpawnPos;

    private void OnEnable()
    {
        _drawActionRef.action.Enable();
        _drawActionRef.action.performed += OnDrawPerformed;
        _drawActionRef.action.canceled += OnDrawCanceld;
    }

    private void OnDrawPerformed(InputAction.CallbackContext ctx)
    {
        float dist = Vector3.Distance(_tip.position, _lastSpawnPos);

        if (_currentStroke == null || dist >= _segmentLength)
        {
            // StrokeをSpawnしてTipに追従させる
            _currentStroke = Runner.Spawn(_strokePrefab, _tip.position, Quaternion.identity).gameObject;
            _currentStroke.transform.SetParent(_tip); // ペン先に追従
            _lastSpawnPos = _tip.position;
        }
    }

    private void OnDrawCanceld(InputAction.CallbackContext ctx)
    {
        if (_currentStroke != null)
        {
            _currentStroke.transform.SetParent(null); // 固定して残す
            _currentStroke = null;
        }
    }
}
