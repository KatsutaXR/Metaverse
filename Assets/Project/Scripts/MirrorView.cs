using UnityEngine;

public class MirrorView : MonoBehaviour
{
    private Transform _playerCamera;
    public Transform PlayerCamera
    {
        get { return _playerCamera; }
        set { _playerCamera = value; }
    }
    [SerializeField] Transform _mirror;
    [SerializeField] Transform _mirrorCamera;
    void Update()
    {
        if (_playerCamera == null) return;
        Vector3 localPlayer = _mirror.InverseTransformPoint(_playerCamera.position);
        _mirrorCamera.position = _mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));
    
        Vector3 lookAtMirror = _mirror.TransformPoint(new Vector3(-localPlayer.x, -localPlayer.y, localPlayer.z));
        _mirrorCamera.LookAt(lookAtMirror);
    }
}
