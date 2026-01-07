using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(AttachChildren))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRGrabInteractable))]
public class BigPoster : MonoBehaviour
{
    public XRGrabInteractable grabInteractable;
    public Rigidbody rb;
    public LayerMask wallLayer;
    public float snapOffset = 0.01f;

    public Transform snapPoint;

    void Awake()
    {
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        grabInteractable.selectExited.AddListener(OnReleased);

        // Désactivé au départ
        grabInteractable.enabled = false;
        rb.isKinematic = true;
    }

    // Active le grab et initialise AttachChildren
    public void EnableGrab()
    {
        GetComponent<AttachChildren>().InitializeOffsets();
        grabInteractable.enabled = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        SnapToWall();
    }

private void SnapToWall()
{
    if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5f, wallLayer))
    {
        Debug.Log("Pas de mur détecté pour snap");
        return;
    }

    if (snapPoint == null)
    {
        Debug.LogWarning("SnapPoint non assigné !");
        return;
    }

    // Calcul de l'offset entre le pivot et la face avant
    Vector3 pivotToFront = snapPoint.position - transform.position;

    // Position finale : le mur + offset
    transform.position = hit.point - pivotToFront + hit.normal * snapOffset;

    // Rotation : aligner l'axe Z local vers l’opposé de la normale du mur
    transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

    // Reset physique
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    Debug.Log("BigPoster snap au mur à " + transform.position);
}
}
