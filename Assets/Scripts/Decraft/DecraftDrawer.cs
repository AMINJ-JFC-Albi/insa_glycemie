using UnityEngine;

namespace Decraft {
    public class DecraftDrawer : MonoBehaviour {
        private DecraftManager decraftManager;
        [SerializeField] private DecraftType decraftType;
        
        void Awake() {
            decraftManager = GetComponentInParent<DecraftManager>();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out DecraftObject otherDecraftObject)) {
                if (decraftType == otherDecraftObject.decraftType) {
                    decraftManager.Add();
                }
            }
        }
        
        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out DecraftObject otherDecraftObject)) {
                if (decraftType == otherDecraftObject.decraftType) {
                    decraftManager.Remove();
                }
            }
        }
    }
}
