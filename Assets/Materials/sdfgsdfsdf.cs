using UnityEngine;

public class sdfgsdfsdf : MonoBehaviour {
    public Transform playerTarget;
    public Transform mirror;
    
    void Update() {
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        transform.position = mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        Vector3 lookatmirror = mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(mirror);
    }
}
