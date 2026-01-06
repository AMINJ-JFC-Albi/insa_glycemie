using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Cryptex {
    [RequireComponent(typeof(XRBaseInteractable))]
    public class CryptexWheelInteractor : MonoBehaviour {
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private float rotationSensitivity = 1.0f;

        private XRBaseInteractor currentInteractor;
        private bool isHovering;

        private float startWheelAngle;
        private float startHandAngle;

        private CryptexWheel cryptexWheel;
        private XRSimpleInteractable interactable;

        private void Awake() {
            if (wheelTransform == null) {
                wheelTransform = transform;
            }

            cryptexWheel = GetComponent<CryptexWheel>();
            interactable = GetComponent<XRSimpleInteractable>();

            interactable.selectEntered.AddListener(OnHoverEntered);
            interactable.selectExited.AddListener(OnHoverExited);
        }

        private void OnDestroy() {
            interactable.selectEntered.RemoveListener(OnHoverEntered);
            interactable.selectExited .RemoveListener(OnHoverExited);
        }

        private void Update() {
            if (!isHovering || !currentInteractor) {
                return;
            }

            Transform parent = wheelTransform.parent;
            Transform hand = currentInteractor.transform;

            Vector3 localHandPos = parent.InverseTransformPoint(hand.position);
            Vector3 localWheelPos = wheelTransform.localPosition;

            Vector3 dir = localHandPos - localWheelPos;
            dir.y = 0.0f;

            if (dir.sqrMagnitude < 0.0001f) {
                return;
            }

            float currentHandAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float delta = currentHandAngle - startHandAngle;

            float finalAngle = startWheelAngle + delta * rotationSensitivity;

            wheelTransform.localRotation = Quaternion.Euler(0.0f, finalAngle, 0.0f);
        }

        private void OnHoverEntered(SelectEnterEventArgs args) {
            XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
            if (interactor == null) {
                return;
            }

            currentInteractor = interactor;
            isHovering = true;

            startWheelAngle = wheelTransform.localEulerAngles.y;

            Transform parent = wheelTransform.parent;
            Vector3 localHandPos = parent.InverseTransformPoint(interactor.transform.position);
            Vector3 localWheelPos = wheelTransform.localPosition;

            Vector3 dir = localHandPos - localWheelPos;
            dir.y = 0.0f;
            dir.Normalize();

            startHandAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }

        private void OnHoverExited(SelectExitEventArgs args) {
            XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
            if (interactor != currentInteractor) {
                return;
            }

            isHovering = false;
            currentInteractor = null;

            if (cryptexWheel != null) {
                //cryptexWheel.OnRelease();
            }
        }
    }
}
