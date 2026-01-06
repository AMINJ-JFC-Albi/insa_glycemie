using NavKeypad;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Keypad {
    public class ButtonFollowVisual : MonoBehaviour {
        [SerializeField] private Transform visualTarget;
        [SerializeField] private Vector3 localAxis;

        private bool freeze;
        private Vector3 initialLocalPos;

        private Vector3 offset;
        private Transform pokeAttachTransform;

        private XRBaseInteractable interactable;
        private bool isFollowing;

        private void Start() {
            interactable = GetComponent<XRBaseInteractable>();
            visualTarget = transform.parent.GetChild(1);
            initialLocalPos = visualTarget.localPosition;

            interactable.hoverEntered.AddListener(Follow);
            interactable.hoverExited.AddListener(StopFollowing);
            interactable.selectEntered.AddListener(Freeze);
        }

        private void Update() {
            if (freeze) return;

            if (isFollowing) {
                Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
                Vector3 constrainedLocalPosition = Vector3.Project(localTargetPosition, localAxis);
                visualTarget.position = visualTarget.TransformPoint(constrainedLocalPosition);
            } else {
                visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPos, Time.deltaTime * 10f);
            }
        }

        private void Follow(BaseInteractionEventArgs hover) {
            if (hover.interactorObject is XRPokeInteractor interactor) {
                pokeAttachTransform = interactor.attachTransform;
                offset = visualTarget.position - pokeAttachTransform.position;
                float pokeAngle = Vector3.Angle(offset, visualTarget.TransformDirection(localAxis));
                if (pokeAngle < 45f) {
                    isFollowing = true;
                    freeze = false;
                }
            }
        }

        private void StopFollowing(BaseInteractionEventArgs hover) {
            if (hover.interactorObject is XRPokeInteractor) {
                isFollowing = false;
                freeze = false;
            }
        }

        private void Freeze(BaseInteractionEventArgs hover) {
            if (hover.interactorObject is XRPokeInteractor) {
                freeze = true;
                KeypadButton keypadButton = transform.parent.GetComponent<KeypadButton>();
                if (keypadButton) {
                    keypadButton.PressButton();
                } else {
                    // C'est le tapeRecorder
                    Debug.Log("audio !!!");
                    //PLAY AUDIO !!! (que faire si rappuis ? Relancer au début, rien tant que déjà en cours, arrêter...)
                }
            }
        }
    }
}
