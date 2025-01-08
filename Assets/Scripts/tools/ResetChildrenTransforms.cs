using System.Collections.Generic;
using UnityEngine;

public class ResetChildrenTransforms : MonoBehaviour
{
    private class TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }

    private Dictionary<Transform, TransformData> initialTransforms = new Dictionary<Transform, TransformData>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            initialTransforms[child] = new TransformData(child.localPosition, child.localRotation, child.localScale);
        }

        ResetObjects();
    }

    public void ResetObjects()
    {
        foreach (var entry in initialTransforms)
        {
            if (entry.Key != null && entry.Key.gameObject.activeInHierarchy)
            {
                entry.Key.SetLocalPositionAndRotation(entry.Value.Position, entry.Value.Rotation);
                entry.Key.localScale = entry.Value.Scale;
                if (entry.Key.TryGetComponent<Rigidbody>(out var rb))
                {
                    if (!rb.isKinematic)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }
}
