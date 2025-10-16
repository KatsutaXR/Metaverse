using Fusion;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NetworkObjectGrabController : NetworkBehaviour
{
    private XRBaseInteractor _xrBaseinteractor;
    private bool _isStateRequesting = false;
    private bool _isGrabbingThisObj = false;
    public bool IsGrabbingThisObj => _isGrabbingThisObj;

    public override void Render()
    {
        _isGrabbingThisObj = CheckGrabAndState();
    }

    private bool CheckGrabAndState()
    {
        if (_xrBaseinteractor == null)
        {
            _xrBaseinteractor = FindFirstObjectByType<PlayerReferences>()?.RightNearFarInteractor;
            if (_xrBaseinteractor == null) return false;
        }

        if (_xrBaseinteractor.firstInteractableSelected == null) return false;

        var grabbed = _xrBaseinteractor.firstInteractableSelected?.transform.gameObject;
        if (grabbed != gameObject) return false;

        if (!HasStateAuthority && !_isStateRequesting)
        {
            Object.RequestStateAuthority();
            _isStateRequesting = true;
        }
        else if (HasStateAuthority && _isStateRequesting)
        {
            _isStateRequesting = false;
        }

        return true;
    }

}
