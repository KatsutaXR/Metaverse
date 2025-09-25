using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NetworkObjectGrabController : NetworkBehaviour
{
    private XRBaseInteractor _xrBaseinteractor;
    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
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
    }

}
