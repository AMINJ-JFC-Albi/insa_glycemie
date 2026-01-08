using UnityEngine;

namespace Decraft {
    public class DecraftManager : MonoBehaviour {
        [SerializeField] private ConfigurableJoint targetJoint;
        private int currentCount;
        
        public void Add() {
            currentCount++;
            if (currentCount >= 4) {
                targetJoint.zMotion = ConfigurableJointMotion.Limited;
            }
        }
        
        public void Remove() {
            currentCount--;
        }
    }
}
