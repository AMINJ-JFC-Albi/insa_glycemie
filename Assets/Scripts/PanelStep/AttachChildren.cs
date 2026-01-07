using System.Collections.Generic;
using UnityEngine;

public class AttachChildren : MonoBehaviour
{
    private List<Transform> children = new List<Transform>();
    private Vector3[] localPositions;
    private Quaternion[] localRotations;
    private bool initialized = false;

    // Ajoute un enfant si ce n'est pas déjà fait
    public void AddChildIfNotAlready(Transform child)
    {
        if (!children.Contains(child))
            children.Add(child);
    }

    // Initialise les offsets et rotations locales
    public void InitializeOffsets()
    {
        localPositions = new Vector3[children.Count];
        localRotations = new Quaternion[children.Count];

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == null) continue;

            // Convertit la position de l'enfant en local space
            localPositions[i] = transform.InverseTransformPoint(children[i].position);
            // Convertit la rotation de l'enfant en rotation relative au parent
            localRotations[i] = Quaternion.Inverse(transform.rotation) * children[i].rotation;
        }

        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized) return;

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == null) continue;

            // Applique la position et rotation locales transformées en world space
            children[i].position = transform.TransformPoint(localPositions[i]);
            children[i].rotation = transform.rotation * localRotations[i];
        }
    }
}
