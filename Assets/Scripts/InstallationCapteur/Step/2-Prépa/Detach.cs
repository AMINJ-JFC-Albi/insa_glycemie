using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Detach : MonoBehaviour
{
    [Header("Références")]
    public XRGrabInteractable bottleGrab; // XRGrabInteractable de la bouteille
    public XRGrabInteractable capGrab;    // XRGrabInteractable du bouchon
    public Rigidbody capRigidbody;        // Rigidbody du bouchon

    private bool capOnBottle = true;
    private Vector3 localOffset;
    private Quaternion rotationOffset;

    public TaskManager taskManager;


    void Start()
    {
        // Calcul offset local par rapport à la bouteille
        localOffset = bottleGrab.transform.InverseTransformPoint(capGrab.transform.position);
        rotationOffset = Quaternion.Inverse(bottleGrab.transform.rotation) * capGrab.transform.rotation;

        // Physique initiale
        capRigidbody.isKinematic = false;   // ne pas mettre true pour éviter l’erreur
        capRigidbody.useGravity = false;

        // Grab désactivé au départ
        capGrab.enabled = false;

        // Désactiver Throw On Detach pour éviter l’erreur angularVelocity
        capGrab.throwOnDetach = false;

        // Stabilisation quand grabé
        capRigidbody.linearDamping = 100f;
        capRigidbody.angularDamping = 100f;
    }

    void Update()
    {
        if (capOnBottle)
        {
            // Tant que le bouchon n'est pas grabé, suivre la bouteille
            if (!capGrab.isSelected)
            {
                capGrab.transform.position = bottleGrab.transform.TransformPoint(localOffset);
                capGrab.transform.rotation = bottleGrab.transform.rotation * rotationOffset;

                // Activer grab si la bouteille est grabée
                capGrab.enabled = bottleGrab.isSelected;
            }

            // Quand le bouchon est grabé
            if (capGrab.isSelected)
            {
                capOnBottle = false;
                taskManager?.NextStep();

                // Le Rigidbody reste non kinematic mais fortement stabilisé
                capRigidbody.useGravity = false;
                capRigidbody.linearDamping = 100f;
                capRigidbody.angularDamping = 100f;

                // Pas de parent, XRGrabInteractable gère la position
                capGrab.transform.parent = null;
            }
        }
        else
        {
            // Si le bouchon est lâché après grab, réactiver la physique normale
            if (!capGrab.isSelected)
            {
                capRigidbody.useGravity = true;
                capRigidbody.linearDamping = 0.5f;
                capRigidbody.angularDamping = 0.05f;
            }

        }
    }
}
