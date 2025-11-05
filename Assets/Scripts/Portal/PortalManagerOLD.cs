using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace PortalOLD {
    public class PortalManagerOLD : MonoBehaviour {
        private static PortalManagerOLD instance;
        public static PortalManagerOLD Instance {
            get {
                if (!instance) instance = FindFirstObjectByType<PortalManagerOLD>();
                return instance;
            }
        }

        private Transform playerCamera;
        private Transform mainPortal;
        private Transform secondaryPortal;

        [SerializeField] private Camera portalCamera;
        private Transform portalCameraTransform;
        [SerializeField] private Material portalMaterial;
        private static readonly int PortalTexture = Shader.PropertyToID("_PortalTexture");
        public Renderer meshMaskRenderer;

        void Awake() {
            playerCamera = Camera.main?.transform;
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
            Init();
        }

        public void Init() {
            OnePortal[] portals = FindObjectsByType<OnePortal>(FindObjectsSortMode.None);
            foreach (OnePortal portal in portals) {
                if (!portal) continue;
                if (portal.IsMainPortal()) {
                    mainPortal = portal.transform;
                } else {
                    secondaryPortal = portal.transform;
                }
            }
            portalCameraTransform = portalCamera.transform;

            StartCoroutine(TextureAndFollow());
        }

        private IEnumerator TextureAndFollow() {
            if (!playerCamera || !mainPortal || !secondaryPortal) {
                Debug.LogError("PortalManager: Missing references for portal following.");
            }

            // Attente de l'initialisation de XR
            while (XRSettings.eyeTextureWidth == 0 || XRSettings.eyeTextureHeight == 0) {
                yield return null;
            }

            // Mise en place du Render Texture
            if (portalCamera.targetTexture) {
                portalCamera.targetTexture.Release();
            }
            portalCamera.targetTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
            //portalMaterial.mainTexture = portalCamera.targetTexture;
            portalMaterial.SetTexture(PortalTexture, portalCamera.targetTexture);

            // Boucle pour la position et rotation de la caméra du portail
            while (true) {
                Vector3 posInMainPortalSpace = mainPortal.InverseTransformPoint(playerCamera.position);
                posInMainPortalSpace = new Vector3(-posInMainPortalSpace.x, posInMainPortalSpace.y, -posInMainPortalSpace.z);
                portalCameraTransform.position = secondaryPortal.TransformPoint(posInMainPortalSpace);

                Vector3 dirInMainPortalSpace = mainPortal.InverseTransformDirection(playerCamera.forward);
                Vector3 upInMainPortalSpace = mainPortal.InverseTransformDirection(playerCamera.up);
                Vector3 newForward = secondaryPortal.TransformDirection(new Vector3(-dirInMainPortalSpace.x, dirInMainPortalSpace.y, -dirInMainPortalSpace.z));
                Vector3 newUp = secondaryPortal.TransformDirection(new Vector3(-upInMainPortalSpace.x, upInMainPortalSpace.y, -upInMainPortalSpace.z));
                portalCameraTransform.rotation = Quaternion.LookRotation(newForward, newUp);

                yield return null;
            }
        }
    }
}
