using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRSocketInteractorTag : XRSocketInteractor {
    public string targetTag;
    
    /*
    public override bool CanSelect(IXRSelectInteractable interactable) {
        return base.CanSelect(interactable) && interactable
            .CompareTag(targetTag);
    }*/
}
    