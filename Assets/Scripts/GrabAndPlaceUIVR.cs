using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[System.Serializable]
public class Slot
{
    public Transform slotTransform;      // Où placer l'affiche
    public GameObject requiredObject;    // Quelle affiche doit être dans ce slot
}

public class GrabAndPlaceUIVR : MonoBehaviour
{
    [Header("XR Grab Interactable")]
    public XRGrabInteractable grabInteractable;

    [Header("Slots Configuration")]
    public Slot[] slots;

    [Header("Interaction Layers")]
    public InteractionLayerMask grabableLayers;
    public InteractionLayerMask nonGrabableLayers;

    [Header("Wall Placement")]
    public LayerMask wallLayer;
    public float snapOffset = 0.01f;
    public bool autoSnapOnRelease = true;

    private Rigidbody rb;
    private bool slotsValid = false;  // Stop Update après validation

    private Vector3 initialPos;
    private Quaternion initialRot;

    void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        grabInteractable.selectExited.AddListener(OnRelease);

        initialPos = transform.position;
        initialRot = transform.rotation;

        SnapToNearestWall();
    }

    void Update()
    {
        if (slotsValid) return;

        bool allFilled = true;

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            bool slotFilled = false;

            if (slot.slotTransform != null && slot.slotTransform.childCount > 0)
            {
                GameObject placedObject = slot.slotTransform.GetChild(0).gameObject;
                if (placedObject == slot.requiredObject)
                    slotFilled = true;

                Debug.Log($"Slot {i} : {placedObject.name} {(slotFilled ? "correct" : "incorrect")}");
            }
            else
            {
                Debug.Log($"Slot {i} : vide");
            }

            if (!slotFilled)
                allFilled = false;
        }

        grabInteractable.interactionLayers = allFilled ? grabableLayers : nonGrabableLayers;
        Debug.Log("Tous les slots remplis : " + allFilled);

        if (allFilled)
        {
            slotsValid = true;
            Debug.Log("Tous les slots validés, Update arrêté");
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        bool placedCorrectly = false;
        foreach (var slot in slots)
        {
            if (slot.slotTransform != null && slot.slotTransform.childCount > 0)
            {
                if (slot.slotTransform.GetChild(0).gameObject == gameObject &&
                    slot.requiredObject == gameObject)
                {
                    placedCorrectly = true;
                    break;
                }
            }
        }

        if (!placedCorrectly)
        {
            transform.position = initialPos;
            transform.rotation = initialRot;
        }

        if (autoSnapOnRelease)
            SnapToNearestWall();
    }

    void SnapToNearestWall()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f, wallLayer))
        {
            transform.position = hit.point + hit.normal * snapOffset;
            transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            Debug.Log("Snap au mur effectué à " + hit.point);
        }
        else
        {
            rb.isKinematic = true;
            Debug.Log("Pas de mur détecté pour snap");
        }
    }

    public void PlaceInSlot(Slot slot)
    {
        if (slot.requiredObject != gameObject)
        {
            transform.position = initialPos;
            transform.rotation = initialRot;
            return;
        }

        transform.SetParent(slot.slotTransform);
        transform.localPosition = new Vector3(0, 0, -0.001f);
        transform.localRotation = Quaternion.identity;
    }
}
