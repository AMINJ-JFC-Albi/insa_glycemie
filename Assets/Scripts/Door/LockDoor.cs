using System.Collections.Generic;
using HoloWatch;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Door {
    public class LockDoor : MonoBehaviour {
        private XRGrabInteractable grab;
        private XRSocketInteractor socket;
        private Rigidbody rb;

        public UnityEvent canTp;

        private void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            socket = GetComponentInChildren<XRSocketInteractor>();
            rb = GetComponent<Rigidbody>();
            grab.enabled = false;

            socket.selectEntered.AddListener(KeyInLock);
        }

        private void KeyInLock(SelectEnterEventArgs arg0) {
            StartCoroutine(arg0.interactableObject.transform.GetComponent<Key>().IsInLock(socket, this));
        }

        private bool firstUnlock = true;
        public void UnlockDoor() {
            grab.enabled = true;
            rb.isKinematic = false;
            if (firstUnlock) {
                HolowatchUI.Instance.CompleteObjective("Enter");
                HolowatchUI.Instance.StartObjective("Door");
                HolowatchUI.Instance.SetNextHints("Door", new List<string>() { "2-1", "2-2" });
            } else {
                HolowatchUI.Instance.SetNextHints("Badge", new List<string>() { "4-4" });
            }
            canTp.Invoke();
        }
    }
}
