using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnapToSlot : MonoBehaviour
{
    public Transform targetSlot;
    public float snapDistance = 0.15f;

    XRGrabInteractable grab;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grab.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (Vector3.Distance(transform.position, targetSlot.position) < snapDistance)
        {
            transform.SetParent(targetSlot);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            rb.isKinematic = true;
            grab.enabled = false;
        }
    }
}
