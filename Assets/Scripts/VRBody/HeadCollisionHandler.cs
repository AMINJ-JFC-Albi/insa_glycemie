using UnityEngine;
using System.Collections.Generic;

namespace VRBody {
    public class HeadCollisionHandler : MonoBehaviour {
        [SerializeField] private HeadCollisionDetector detector;
        [SerializeField] private CharacterController characterController;
        [SerializeField] public float pushBackStrength = 1.0f;

        private Vector3 CalculatePushBackDirection(List<RaycastHit> colliderHits) {
            Vector3 combinedNormal = Vector3.zero;
            foreach (RaycastHit hitPoint in colliderHits) {
                combinedNormal += new Vector3(hitPoint.normal.x, 0, hitPoint.normal.z);
            }
            return combinedNormal;
        }

        private void Update() {
            if (detector.detectedColliderHits.Count <= 0) {
                return;
            }
            Vector3 pushBackDirection = CalculatePushBackDirection(detector.detectedColliderHits);

            Debug.DrawRay(transform.position, pushBackDirection.normalized, Color.magenta);

            characterController.Move(pushBackDirection.normalized * (pushBackStrength * Time.deltaTime));
        }
    }
}
