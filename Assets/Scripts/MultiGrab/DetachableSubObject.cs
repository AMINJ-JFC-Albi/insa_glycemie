using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace MultiGrab {
    public class DetachableSubObject : MonoBehaviour {
        private XRGrabInteractable grab;
        private Rigidbody rb;
        private Renderer rend;
        private Color[] originalColors;

        public DetachableSubObject prerequisiteObject;
        private bool isDetached;

        private void Awake() {
            grab = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            rend = GetComponent<Renderer>();
            if (rend) {
                originalColors = new Color[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++) {
                    originalColors[i] = rend.materials[i].color;
                }
            }

            grab.enabled = false;
            rb.isKinematic = true;
            
            grab.selectEntered.AddListener(OnGrabbed);
            grab.hoverEntered.AddListener(OnHoverEnter);
            grab.hoverExited.AddListener(OnHoverExit);
        }

        public void EnableDetach(bool canEnabled) {
            if (isDetached || (prerequisiteObject != null && !prerequisiteObject.isDetached)) {
                return;
            }
            grab.enabled = canEnabled;
        }

        private void OnGrabbed(SelectEnterEventArgs args) {
            if (isDetached) return;
            isDetached = true;

            // Force XR à lâcher toute relation précédente
            XRInteractionManager interactionManager = grab.interactionManager;
            IXRSelectInteractor interactor = args.interactorObject;
            interactionManager.SelectExit(interactor, grab);

            rb.isKinematic = false;
            
            transform.SetParent(null);
            grab.enabled = true;
            gameObject.layer = LayerMask.NameToLayer("Default");

            if (rend) {
                for (int i = 0; i < rend.materials.Length; i++) {
                    rend.materials[i].color = originalColors[i];
                }
            }

            interactionManager.SelectEnter(interactor, grab);
        }

        private void OnHoverEnter(HoverEnterEventArgs args) {
            if (!isDetached && rend && (prerequisiteObject == null || prerequisiteObject.isDetached)) {
                // Rendre chaque matériau un peu bleuté et transparent
                for (int i = 0; i < rend.materials.Length; i++) {
                    Color c = originalColors[i];
                    c.r = 0.5f;
                    c.g = 0.7f;
                    c.b = 0.7f;
                    rend.materials[i].color = c;
                }
            }
        }

        private void OnHoverExit(HoverExitEventArgs args) {
            if (!isDetached && rend && (prerequisiteObject == null || prerequisiteObject.isDetached)) {
                // Restaurer les couleurs originales
                for (int i = 0; i < rend.materials.Length; i++) {
                    rend.materials[i].color = originalColors[i];
                }
            }
        }
    }
}