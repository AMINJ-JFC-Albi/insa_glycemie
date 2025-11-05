using UnityEngine;

namespace PortalOLD {
    public class OnePortal : MonoBehaviour {
        [SerializeField] private bool isMainPortal;
        
        private Shader portalShader;
        [SerializeField] private MeshRenderer portalRenderer;
        private static readonly int PortalTexture = Shader.PropertyToID("_PortalTexture");

        public bool IsMainPortal() {
            return isMainPortal;
        }
        
        public void Activate(Shader shader, Transform otherPortalTransform, bool isThisSide) {
            portalShader = shader;
            GetComponentInChildren<OnePortalCamera>().Activate(otherPortalTransform, isThisSide);
        }
        
        public void SetTexture(Camera cam) {
            Material portalMaterial = new Material(portalShader);
            portalMaterial.SetTexture(PortalTexture, cam.targetTexture);
            portalRenderer.material = portalMaterial;
        }
    }
}
