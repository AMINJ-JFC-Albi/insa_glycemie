using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    [System.Serializable]
    public class ColliderPair
    {
        public Collider collider1;
        public Collider collider2;
    }

    public ColliderPair[] colliderPairs;

    void Start()
    {
        foreach (var pair in colliderPairs)
        {
            if (pair.collider1 != null && pair.collider2 != null)
            {
                Physics.IgnoreCollision(pair.collider1, pair.collider2);
                Debug.Log($"Collision désactivée entre {pair.collider1.name} et {pair.collider2.name}");
            }
            else
            {
                Debug.LogWarning("Une paire de colliders contient un ou plusieurs objets null.");
            }
        }
    }
}
