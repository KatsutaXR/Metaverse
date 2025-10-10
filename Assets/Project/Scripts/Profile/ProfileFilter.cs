using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ProfileFilter : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
{
    public bool canProcess => isActiveAndEnabled;
    public GameObject ClientUI { get; set; }
    public bool IsMenuOpen => ClientUI.activeSelf;

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        if (interactable.transform.gameObject.tag != "Profile") return true;

        return IsMenuOpen;
    }

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        if (interactable.transform.gameObject.tag != "Profile") return true;

        return IsMenuOpen;
    }
}
