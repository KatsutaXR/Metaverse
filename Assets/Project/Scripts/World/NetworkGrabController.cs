using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NetworkObjectGrabController : NetworkBehaviour
{
    private XRBaseInteractor _xrBaseinteractor;
    private bool _isStateRequesting;
    public override void Spawned()
    {
        if (Runner.IsSharedModeMasterClient) Object.ReleaseStateAuthority();
        _isStateRequesting = false;
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

        if (!HasStateAuthority && !_isStateRequesting)
        {
            Debug.Log("!HasStateAuthority");
            Object.RequestStateAuthority();
            _isStateRequesting = true;
        }
        else if (HasStateAuthority && _isStateRequesting)
        {
            _isStateRequesting = false;
        }
    }

}
