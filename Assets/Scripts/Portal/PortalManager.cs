using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Portal {
    public class PortalManager : MonoBehaviour {
        private static PortalManager instance;
        public static PortalManager Instance {
            get {
                if (!instance) instance = FindFirstObjectByType<PortalManager>();
                return instance;
            }
        }

        private Transform playerCamera;
        private Transform mainPortal;
        private Transform secondaryPortal;

        [SerializeField] private Camera portalCamera;
        private Transform portalCameraTransform;
        [SerializeField] private Material portalMaterial;

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

            StartCoroutine(Follow());
        }

        private IEnumerator Follow() {
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
            portalMaterial.mainTexture = portalCamera.targetTexture;

            // Boucle pour la position et rotation de la caméra du portail
            while (true) {
                Vector3 relativePos = playerCamera.position - mainPortal.position;
                portalCameraTransform.position = secondaryPortal.position + relativePos;

                Quaternion relativeRotation = secondaryPortal.rotation * Quaternion.Inverse(mainPortal.rotation);
                Vector3 newCameraDirection = relativeRotation * playerCamera.forward;
                Vector3 newCameraUp = relativeRotation * playerCamera.up;
                portalCameraTransform.rotation = Quaternion.LookRotation(newCameraDirection, newCameraUp);
                
                yield return null;
            }
        }
    }
}
