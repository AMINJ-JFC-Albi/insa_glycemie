using UnityEngine;

namespace Portal {
    public class OnePortal : MonoBehaviour {
        [SerializeField] private bool isMainPortal;
        
        public bool IsMainPortal() {
            return isMainPortal;
        }
    }
}
