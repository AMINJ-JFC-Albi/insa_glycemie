using UnityEngine;

namespace ResetButton {
    public class InitialTransform : MonoBehaviour {
        public Vector3 initialPosition { get; private set; }
        public Quaternion initialRotation { get; private set; }

        public void Awake() {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }
    }
}
