using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToTeleport;
    [SerializeField] private GameObject destination;

    public void Teleport()
    {
        objectToTeleport.transform.parent = destination.transform;
        objectToTeleport.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (objectToTeleport.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        objectToTeleport.SetActive(true);
    }
}
