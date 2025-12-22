using UnityEngine;
using System.Collections;

public class attachCGM : MonoBehaviour
{
    [Header("Enfant à coller")]
    public GameObject enfantAFixer;   // L'enfant du parent à coller
    [Header("Parent grabbable")]
    public GameObject parentGrab;     // L'objet tenu dans la main

    private bool dejaPlace = false;

    void OnCollisionEnter(Collision collision)
    {
        if (dejaPlace) return;
        if (collision.rigidbody == null) return;

        ContactPoint contact = collision.contacts[0];

        // 1️⃣ Instancier le clone indépendant
        GameObject clone = Instantiate(enfantAFixer);
        clone.transform.SetParent(null, true); // retirer toute hiérarchie
        clone.transform.position = contact.point;

        // 2️⃣ Rotation globale alignée avec la surface
        clone.transform.rotation = Quaternion.FromToRotation(Vector3.up, contact.normal) * clone.transform.rotation;

        // 3️⃣ Ajouter Rigidbody kinematic
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb == null) rb = clone.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // 4️⃣ Ajouter FixedJoint pour fixation définitive
        FixedJoint joint = clone.AddComponent<FixedJoint>();
        joint.connectedBody = collision.rigidbody;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        // 5️⃣ Désactiver le collider et supprimer l'objet original pour ne pas bloquer le parent
        StartCoroutine(DestroyOriginalNextFrame());

        dejaPlace = true;

        Debug.Log("Clone fixé, parent grab reste intact et manipulable.");
    }

    IEnumerator DestroyOriginalNextFrame()
    {
        yield return null; // attendre la fin de la frame

        if (enfantAFixer != null)
        {
            // Désactiver le collider pour ne pas bloquer le parent
            Collider col = enfantAFixer.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Détruire l'enfant collé
            Destroy(enfantAFixer);
        }
    }
}
