using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace PortalOLD {
    public class PortalManager : MonoBehaviour {
        private static PortalManager instance;
        public static PortalManager Instance {
            get {
                if (!instance) instance = FindFirstObjectByType<PortalManager>();
                return instance;
            }
        }
        
        [SerializeField] private Shader portalShader;
        [SerializeField] private OnePortal portal1;
        [SerializeField] private OnePortal portal2;

        void Awake() {
            if (instance != null && instance != this) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy() {
            if (instance == this) instance = null;
        }

        private void Start() {
            portal1.Activate(portalShader, portal2.transform, false);
            portal2.Activate(portalShader, portal1.transform, true);
        }
    }
}
