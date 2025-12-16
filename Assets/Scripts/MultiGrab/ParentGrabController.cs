using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MultiGrab {
    public class ParentGrabController : MonoBehaviour {
        private XRGrabInteractable grab;
        private DetachableSubObject[] subObjects;

        private void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            subObjects = transform.parent.GetComponentsInChildren<DetachableSubObject>(true);

            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }

        private void Start() {
            foreach (DetachableSubObject subObject in subObjects) {
                subObject.transform.SetParent(transform);
            }
        }

        private void OnGrab(SelectEnterEventArgs args) {
            foreach (DetachableSubObject sub in subObjects) {
                sub.EnableDetach(true);
            }
        }

        private void OnRelease(SelectExitEventArgs args) {
            foreach (DetachableSubObject sub in subObjects) {
                sub.EnableDetach(false);
            }
        }
    }
}