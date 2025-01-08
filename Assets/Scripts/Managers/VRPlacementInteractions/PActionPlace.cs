using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PActionPlace : IPlacementAction
{
    [SerializeField] private bool removeHeldObject = true, copySelfTransform = true, freeze = true, removeXRGrabInteractable = true, removeColliders = true;
    [SerializeField] private GameObject replaceObject;

    public override void Execute(params object[] args)
    {
        if (args.Length < 3)
        {
            Debug.LogError("Execute requires at least 3 arguments: heldObject, placementObject, parent.");
            return;
        }

        // Validation des arguments
        if (args[0] is not GameObject heldObject ||
            args[1] is not GameObject placementObject ||
            args[2] is not Transform parent)
        {
            Debug.LogError("Invalid argument types passed to Execute.");
            return;
        }

        InstantiateAndParamGameObject((replaceObject != null)?replaceObject:heldObject, copySelfTransform?transform:placementObject.transform, parent, freeze);

        if (removeHeldObject) Destroy(heldObject);
    }

    private void InstantiateAndParamGameObject(GameObject gameObjectToCopy, Transform placementPoint, Transform parent, bool freeze)
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent transform is null. Instantiated object will not be parented.");
        }

        GameObject instantiatedObject = Instantiate(
            gameObjectToCopy,
            placementPoint.position,
            placementPoint.rotation,
            parent
        );

        instantiatedObject.transform.localScale = placementPoint.localScale;

        if (freeze && instantiatedObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (removeXRGrabInteractable && instantiatedObject.TryGetComponent<XRGrabInteractable>(out var grab))
        {
            Destroy(grab);
        }

        if (removeColliders)
        {
            foreach (var collider in instantiatedObject.GetComponentsInChildren<Collider>())
            {
                Destroy(collider);
            }
        }
    }
}
