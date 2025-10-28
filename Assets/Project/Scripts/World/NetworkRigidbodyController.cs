using Fusion;
using UnityEngine;

public class NetworkRigidbodyController : NetworkBehaviour
{
    private bool _lastHasState = false;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }
    public override void Render()
    {
        if (_lastHasState != Object.HasStateAuthority)
        {
            _rigidbody.isKinematic = !Object.HasStateAuthority;
        }
    }
    
    
}
