using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Decraft {
    public class DecraftManager : MonoBehaviour {
        [SerializeField] private ConfigurableJoint targetJoint;
        private int currentCount;

        public XRGrabInteractable key;

        void Start()
        {
            key.enabled = false;
        }
        
        public void Add() {
            currentCount++;
            if (currentCount >= 4) {
                targetJoint.zMotion = ConfigurableJointMotion.Limited;
                key.enabled = true;
            }
        }
        
        public void Remove() {
            currentCount--;
        }
    }
}
