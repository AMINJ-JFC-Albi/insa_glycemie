using System.Collections.Generic;
using UnityEngine;

public class UnifiedColliderEventManager : MonoBehaviour
{
    public List<Collider> Colliders;
    private HashSet<Collider> activeObjects = new HashSet<Collider>();

    public delegate void TriggerEvent(Collider collider, GameObject selfGO);
    public event TriggerEvent OnTriggerEnterEvent;
    public event TriggerEvent OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "SM_Chair") return; //TODO Patch with layers Colliders
        if (activeObjects.Count == 0)
        {
            OnTriggerEnterEvent?.Invoke(other, gameObject);
        }
        activeObjects.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!activeObjects.Contains(other))
            return;

        activeObjects.Remove(other);

        if (activeObjects.Count == 0)
        {
            OnTriggerExitEvent?.Invoke(other, gameObject);
        }
    }

    internal void SetColliders(List<Collider> colliders)
    {
        Colliders = colliders;
    }

    internal void AddCollider(Collider collider)
    {
        Colliders ??= new();
        Colliders.Add(collider);
    }
}
