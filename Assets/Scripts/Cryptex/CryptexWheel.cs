using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Cryptex {
    public class CryptexWheel : MonoBehaviour {
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private int letterCount = 26;

        private XRGrabInteractable grab;
        private float anglePerLetter;
        private float currentAngle;

        private void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            anglePerLetter = 360.0f / letterCount;

            grab.selectExited.AddListener(OnRelease);
        }

        public void OnRelease(SelectExitEventArgs args) {
            currentAngle = GetSignedAngle();
            SnapToNearestLetter();
        }

        private void SnapToNearestLetter() {
            float snappedAngle = Mathf.Round(currentAngle / anglePerLetter) * anglePerLetter;

            ApplyAngle(snappedAngle);
            currentAngle = snappedAngle;
        }

        private float GetSignedAngle() {
            Vector3 localEuler = transform.localEulerAngles;
            float angle = Vector3.Dot(localEuler, rotationAxis);

            while (angle > 180.0f) {
                angle -= 360.0f;
            }
            while (angle < -180.0f) {
                angle += 360.0f;
            }

            return angle;
        }

        private void ApplyAngle(float angle) {
            Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
            transform.localRotation = rotation;
        }

        public char GetCurrentLetter() {
            int index = Mathf.RoundToInt(-currentAngle / anglePerLetter);
            index = (index % letterCount + letterCount) % letterCount;

            return (char)('A' + index);
        }
    }
}
