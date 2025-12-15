using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SpecialObjectController : MonoBehaviour {
    [System.Serializable]
    public class PartInfo {
        public string id;
        public SpecialObjectPart part;
    }

    public XRGrabInteractable grabInteractable;
    public List<PartInfo> parts = new List<PartInfo>();

    // quelles mains tiennent quelles parties
    private Dictionary<IXRSelectInteractor, SpecialObjectPart> interactorToPart =
        new Dictionary<IXRSelectInteractor, SpecialObjectPart>();

    private void Reset() {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable() {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable() {
        if (grabInteractable == null)
            return;

        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        // on récupère sur quel collider la main est venue
        var interactor = args.interactorObject;
        var collider = args.interactableObject?.colliders?[0];
        // si tu veux être plus précis, tu peux regarder args.interactorObject.transform.position et un Raycast

        var part = FindPartFromCollider(collider);
        if (part != null) {
            interactorToPart[interactor] = part;
            TryHandleTwoHandLogic();
        } else {
            // main qui attrape "le corps" sans partie spécifique
            interactorToPart[interactor] = null;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args) {
        var interactor = args.interactorObject;
        if (interactorToPart.ContainsKey(interactor))
            interactorToPart.Remove(interactor);
    }

    private SpecialObjectPart FindPartFromCollider(Collider col) {
        if (col == null)
            return null;

        foreach (var p in parts) {
            if (p.part == null)
                continue;

            if (p.part.ContainsCollider(col))
                return p.part;
        }

        return null;
    }

    private void TryHandleTwoHandLogic() {
        // on ne s'intéresse qu’au cas où il y a au moins 2 mains
        if (interactorToPart.Count < 2)
            return;

        // récupérer toutes les paires main/partie
        var list = new List<SpecialObjectPart>(interactorToPart.Values);
        SpecialObjectPart first = list[0];
        SpecialObjectPart second = list[1];

        if (first == null || second == null)
            return;
        if (first == second)
            return;

        // ici : deux mains tiennent deux parties différentes => logique de détachement
        // par exemple on détache la deuxième partie
        DetachPart(second);
    }

    private void DetachPart(SpecialObjectPart part) {
        if (part == null || part.IsDetached)
            return;

        part.DetachFromParent();
    }
}
