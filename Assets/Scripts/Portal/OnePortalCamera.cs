using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace PortalOLD {
    public class OnePortalCamera : MonoBehaviour {
        private Camera cam;
        private Transform camTransform;
        private Transform playerCamera;
        private Transform portal;
        private Transform otherPortal;
        private bool thisSide;
        
        void Awake() {
            cam = GetComponent<Camera>();
            camTransform = cam.transform;
            playerCamera = Camera.main?.transform;
            portal = transform.parent.transform;
        }

        public void Activate(Transform otherPortalTransform, bool isThisSide) {
            otherPortal = otherPortalTransform;
            thisSide = isThisSide;

            if (thisSide) {
                StartCoroutine(TextureAndFollow());
            }
        }
        
        private IEnumerator TextureAndFollow() {
            if (!playerCamera || !otherPortal || !portal) {
                Debug.LogError("PortalManager: Missing references for portal following.");
            }

            // Attente de l'initialisation de XR
            while (XRSettings.eyeTextureWidth == 0 || XRSettings.eyeTextureHeight == 0) {
                yield return null;
            }

            // Mise en place du Render Texture
            if (cam.targetTexture) {
                cam.targetTexture.Release();
            }
            cam.targetTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
            otherPortal.GetComponent<OnePortal>().SetTexture(cam);
            cam.fieldOfView = playerCamera.GetComponent<Camera>().fieldOfView;
            //portalMaterial.mainTexture = portalCamera.targetTexture;
            //portalMaterial.SetTexture(PortalTexture, cam.targetTexture);

            // Boucle pour la position et rotation de la caméra du portail
            while (true) {
                camTransform.position = playerCamera.position;
                camTransform.rotation = playerCamera.rotation;
                
                Vector3 posInMainPortalSpace = otherPortal.InverseTransformPoint(playerCamera.position);
                posInMainPortalSpace = new Vector3(-posInMainPortalSpace.x, posInMainPortalSpace.y, -posInMainPortalSpace.z);
                camTransform.position = portal.TransformPoint(posInMainPortalSpace);
                
                Vector3 dirInMainPortalSpace = otherPortal.InverseTransformDirection(playerCamera.forward);
                Vector3 upInMainPortalSpace = otherPortal.InverseTransformDirection(playerCamera.up);
                Vector3 newForward = portal.TransformDirection(new Vector3(-dirInMainPortalSpace.x, dirInMainPortalSpace.y, -dirInMainPortalSpace.z));
                Vector3 newUp = portal.TransformDirection(new Vector3(-upInMainPortalSpace.x, upInMainPortalSpace.y, -upInMainPortalSpace.z));
                camTransform.rotation = Quaternion.LookRotation(newForward, newUp);

                // Le joueur a traversé le portail
                if (!thisSide) {
                    yield break;
                }
                
                yield return null;
            }
        }
    }
}
