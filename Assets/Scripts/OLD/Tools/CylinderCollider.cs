using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Transform))]
public class CylinderCollider : MonoBehaviour
{
    [Header("Cylinder Settings")]
    [Range(4, 36)] public int segments = 18; // Nombre de segments pour l'approximation
    public float radius = 1.0f; // Rayon du cylindre
    public float height = 2.0f; // Hauteur du cylindre
    public Vector3 center = Vector3.zero; // Position du centre du cylindre
    public float thickness = 0.1f; // Épaisseur des Box Colliders
    public float angleOffset = 15.0f; // Angle de départ de décalage
    public bool isTrigger;
    public bool providesContacts;

    [Header("Debug")]
    public bool showGizmos = true; // Afficher les Gizmos dans l'éditeur

    // Stocker les enfants pour pouvoir les nettoyer si nécessaire
    private GameObject[] colliderChildren;

    private void OnValidate()
    {
        // S'assurer que segments est un multiple de 2
        if (segments % 2 != 0)
        {
            segments += 1; // Ajuste pour être pair
        }

        // Clamp la valeur pour rester dans les limites [4, 36]
        segments = Mathf.Clamp(segments, 4, 36);
    }

    void Awake()
    {
        CreateCylinderCollider(GetComponent<XRGrabInteractable>());
    }

    private void CreateCylinderCollider(XRGrabInteractable grab)
    {
        // Nettoyer les anciens colliders enfants s'ils existent
        foreach (Transform child in transform)
        {
            if (child.CompareTag("CylinderColliderChild"))
            {
                Destroy(child.gameObject);
            }
        }

        colliderChildren = new GameObject[segments];
        float angleStep = 360f / segments;

        UnifiedColliderEventManager ucem = gameObject.AddComponent<UnifiedColliderEventManager>();

        for (int i = 0; i < segments / 2; i++)
        {
            float angle = i * angleStep;

            // Création d'un GameObject enfant pour le Box Collider
            GameObject colliderObj = new("ColliderSegment_" + i);
            colliderObj.transform.parent = transform;
            colliderObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, angle + angleOffset, 0));

            // Ajouter le Box Collider
            BoxCollider boxCollider = colliderObj.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(thickness, height, radius * 2);
            boxCollider.center = center;
            boxCollider.isTrigger = isTrigger;
            boxCollider.providesContacts = providesContacts;

            if (grab != null)
            {
                grab.colliders.Add(boxCollider);
            }

            ucem.AddCollider(boxCollider);

            // Taguer l'objet pour faciliter le nettoyage
            colliderObj.tag = "CylinderColliderChild";

            // Stocker la référence
            colliderChildren[i] = colliderObj;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.green;

        // Affiche le cylindre en Gizmos
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float nextAngle = (i + 1) * angleStep;
            float radian = Mathf.Deg2Rad * angle;
            float nextRadian = Mathf.Deg2Rad * nextAngle;

            Vector3 start = transform.position + center + new Vector3(
                Mathf.Cos(radian) * radius,
                -height / 2,
                Mathf.Sin(radian) * radius
            );

            Vector3 end = transform.position + center + new Vector3(
                Mathf.Cos(nextRadian) * radius,
                -height / 2,
                Mathf.Sin(nextRadian) * radius
            );

            // Dessine le cercle du bas
            Gizmos.DrawLine(start, end);

            // Dessine le cercle du haut
            Gizmos.DrawLine(start + Vector3.up * height, end + Vector3.up * height);

            // Dessine les lignes verticales
            Gizmos.DrawLine(start, start + Vector3.up * height);
        }
    }
#endif

    // Nettoyer les colliders enfants à la destruction
    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("CylinderColliderChild"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}