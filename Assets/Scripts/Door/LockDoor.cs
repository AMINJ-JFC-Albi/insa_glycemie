using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Door {
    public class LockDoor : MonoBehaviour {
        private XRGrabInteractable grab;
        private bool isLocked = true;
        
        private void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            grab.enabled = false;
        }
        
        public void UnlockDoor() {
            isLocked = false;
            grab.enabled = true;
        }
    }
}
