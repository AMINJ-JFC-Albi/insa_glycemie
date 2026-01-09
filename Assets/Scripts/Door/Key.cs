using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Door {
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Key : MonoBehaviour {
        private XRGrabInteractable grab;
        private Rigidbody rb;
        private Collider col;

        void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }

        public IEnumerator IsInLock(XRSocketInteractor socket, LockDoor lockDoor) {
            gameObject.layer = LayerMask.NameToLayer("Not Interactable");
            yield return new WaitForSeconds(0.5f);
            gameObject.layer = LayerMask.NameToLayer("Default");
            
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.throwOnDetach = false;
            col.excludeLayers = LayerMask.GetMask("Door");
            
            HingeJoint joint = gameObject.AddComponent<HingeJoint>();
            joint.anchor = new Vector3(0, 0, 0);
            joint.axis = new Vector3(0, 1, 0);
            
            joint.useLimits = true;
            JointLimits limits = joint.limits;
            limits.min = 0;
            limits.max = 180;
            joint.limits = limits;
            
            yield return new WaitForSeconds(5f);
            
            socket.enabled = false;
            yield return new WaitForSeconds(5f);
            rb.isKinematic = true;
            
            while (!(joint.angle is >= 160f and <= 175f)) {
                yield return null;
            }
            
            grab.enabled = false;
            transform.SetParent(socket.transform.parent, true);
            lockDoor.UnlockDoor();
            //Debug!!! Jouer un son ?
        }
    }
}
