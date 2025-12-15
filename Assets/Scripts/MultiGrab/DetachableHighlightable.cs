using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DetachableHighlightable : MonoBehaviour {
    private XRGrabInteractable grab;
    private Rigidbody rb;
    private Renderer rend;
    private Material[] originalMaterials;

    [Header("Highlight")] public Material highlightMaterial;

    [Header("Détachement")] public bool isDetached = false; // true après détachement

    private void Awake() {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterials = rend.materials;

        // Au départ
        grab.enabled = false;
        rb.isKinematic = true;

        // Événements XR
        grab.selectEntered.AddListener(OnGrabbed);
        grab.hoverEntered.AddListener(OnHoverEnter);
        grab.hoverExited.AddListener(OnHoverExit);
    }

    private void OnGrabbed(SelectEnterEventArgs args) {
        if (isDetached) return;

        isDetached = true;

        // Détachement physique
        transform.SetParent(null);
        rb.isKinematic = false;
        grab.enabled = true;

        // Désactiver throw si tu ne veux pas lancer
        grab.throwOnDetach = false;
    }

    private void OnHoverEnter(HoverEnterEventArgs args) {
        // Surbrillance seulement si attaché
        if (!isDetached && highlightMaterial != null) {
            Material[] mats = new Material[rend.materials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = highlightMaterial;
            rend.materials = mats;
        }
    }

    private void OnHoverExit(HoverExitEventArgs args) {
        if (!isDetached)
            rend.materials = originalMaterials;
    }

    // Optionnel : méthode pour réinitialiser si nécessaire
    public void ResetObject() {
        isDetached = false;
        grab.enabled = false;
        rb.isKinematic = true;
        transform.SetParent(transform.parent); // remets à l’endroit initial
        rend.materials = originalMaterials;
    }
}
