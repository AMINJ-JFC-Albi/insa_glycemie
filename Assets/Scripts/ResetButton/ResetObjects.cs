using UnityEngine;

namespace ResetButton {
    // Reset la position et la rotation des objets à leurs valeurs initiales lors de l'appui du bouton
    public class ResetObjects : MonoBehaviour {
        [SerializeField] private InitialTransform[] objectsToReset;

        public void ResetAllObjects() {
            foreach (InitialTransform objTransform in objectsToReset) {
                ResetObjectPosition(objTransform);
            }
        }

        private static void ResetObjectPosition(InitialTransform objTransform) {
            //DEBUG!!! interdire la téléportation si l'objet est détruit, déjà utilisé ou autre... (si en main ?)
            Transform transform = objTransform.transform;
            Rigidbody rb = objTransform.GetComponent<Rigidbody>();
            
            if (rb != null) {
                rb.isKinematic = true;
                
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                rb.position = objTransform.initialPosition;
                rb.rotation = objTransform.initialRotation;
                
                transform.position = objTransform.initialPosition;
                transform.rotation = objTransform.initialRotation;

                rb.isKinematic = false;
            } else {
                transform.position = objTransform.initialPosition;
                transform.rotation = objTransform.initialRotation;
            }
        }
    }
}
