using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnapToSlot : MonoBehaviour
{
    public Transform targetSlot;      // Slot où snapper la petite affiche
    public float snapDistance = 0.2f; // Distance max pour snap
    public BigPoster bigPoster;       // Référence au BigPoster pour AttachChildren

    public SlotManager slotManager;

    private XRGrabInteractable grab;
    private Rigidbody rb;
    private bool isValid = false;     // Indique si la petite affiche est validée

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grab.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (targetSlot == null) return;

        float distance = Vector3.Distance(transform.position, targetSlot.position);

        if (distance <= snapDistance)
        {
            SnapToTarget();
        }
    }

    // Snappe et valide la petite affiche
    public void SnapToTarget()
    {
        transform.position = targetSlot.position;
        transform.rotation = targetSlot.rotation;

        rb.isKinematic = true;
        rb.useGravity = false;
        grab.enabled = false;

        isValid = true;

        // Ajouter à AttachChildren si BigPoster a le script
        if (bigPoster != null)
        {
            AttachChildren attach = bigPoster.GetComponent<AttachChildren>();
            attach?.AddChildIfNotAlready(transform);
        }

        if (slotManager != null)
        {
            slotManager.ValidateSlots();
        }
    }

    // Méthode pour savoir si cette affiche est validée
    public bool IsValid()
    {
        return isValid;
    }
}
