using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpecialObjectPart : MonoBehaviour {
    [Header("Références")] public Rigidbody mainRigidbody; // rigidbody du SpecialObject
    public XRGrabInteractable mainGrab; // XRGrabInteractable du parent

    [Header("Prefab de partie détachée")] public GameObject detachedPrefab; // optionnel : prefab version indépendante
    public bool IsDetached { get; private set; }

    private Collider[] _colliders;

    private void Awake() {
        _colliders = GetComponentsInChildren<Collider>();
    }

    public bool ContainsCollider(Collider col) {
        if (col == null || _colliders == null)
            return false;

        for (int i = 0; i < _colliders.Length; i++) {
            if (_colliders[i] == col)
                return true;
        }
        return false;
    }

    public void DetachFromParent() {
        if (IsDetached)
            return;
        IsDetached = true;

        if (detachedPrefab != null) {
            // version "propre" : on instancie une nouvelle version grabbable indépendante
            var instance = Instantiate(detachedPrefab, transform.position, transform.rotation);
            // tu peux ajouter XRGrabInteractable + Rigidbody sur le prefab
        }

        // soit on rend cette partie indépendante directement
        transform.SetParent(null);

        // on peut ajouter un Rigidbody si absent
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // on peut aussi ajouter un XRGrabInteractable pour cette partie seule
        var grab = GetComponent<XRGrabInteractable>();
        if (grab == null) {
            grab = gameObject.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.throwOnDetach = true;
        }
    }
}
