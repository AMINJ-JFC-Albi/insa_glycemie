using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChildFollowParent : MonoBehaviour
{
    public Transform parentObj;  // Applicateur
    
    private XRGrabInteractable grabber;
    private Vector3 localOffset;
    private Vector3 originalLocalEuler;  // ← ROTATION Y ORIGINALE FIXE
    private bool offsetCalculated = false;
    
    void Start()
    {
        grabber = parentObj.GetComponent<XRGrabInteractable>();
    }
    
    void LateUpdate()
    {
        if (!offsetCalculated)
        {
            // **POSITION + ROTATION LOCALES ORIGINALES**
            localOffset = parentObj.InverseTransformPoint(transform.position);
            originalLocalEuler = transform.localEulerAngles;
            offsetCalculated = true;
        }
        
        if (parentObj != null)
        {
            // **1. POSITION : OFFSET LOCAL FIXE**
            transform.position = parentObj.TransformPoint(localOffset);
            
            // **2. ROTATION : COMPLÈTEMENT LOCALE (Y FIXE)**
            transform.rotation = parentObj.rotation * Quaternion.Euler(originalLocalEuler);
        }
        
        // **Rigidbody non-kinematic**
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
    }
}
