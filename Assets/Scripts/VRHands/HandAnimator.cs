using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VrHands {
    //Permet de créer des animations de la main (doigts à faire fermer ou ouvrir)
    [RequireComponent(typeof(Animator))]
    public class HandAnimator : MonoBehaviour {
        [SerializeField] private InputActionReference controllerActionGrip;
        [SerializeField] private InputActionReference controllerActionTrigger;
        [SerializeField] private InputActionReference controllerActionPrimary;

        private Animator handAnimator;

        /// <summary>
        /// List of fingers animated when grabbing / using grab action
        /// </summary>
        private readonly List<Finger> grippingFingers = new List<Finger>() {
            new Finger(FingerType.Middle),
            new Finger(FingerType.Ring),
            new Finger(FingerType.Pinky)
        };

        /// <summary>
        /// List of fingers animated when pointing / using trigger action
        /// </summary>
        private readonly List<Finger> pointingFingers = new List<Finger>() {
            new Finger(FingerType.Index)
        };

        /// <summary>
        /// List of fingers animated when locomotion / using thumbstick
        /// </summary>
        private readonly List<Finger> primaryFingers = new List<Finger>() {
            new Finger(FingerType.Thumb)
        };

        private void Start() {
            handAnimator = GetComponent<Animator>();
        }

        private void OnEnable() {
            controllerActionGrip.action.performed += GripAction_performed;
            controllerActionTrigger.action.performed += TriggerAction_performed;
            controllerActionPrimary.action.performed += PrimaryAction_performed;

            controllerActionGrip.action.canceled += GripAction_canceled;
            controllerActionTrigger.action.canceled += TriggerAction_canceled;
            controllerActionPrimary.action.canceled += PrimaryAction_canceled;
        }

        private void OnDisable() {
            controllerActionGrip.action.performed -= GripAction_performed;
            controllerActionTrigger.action.performed -= TriggerAction_performed;
            controllerActionPrimary.action.performed -= PrimaryAction_performed;

            controllerActionGrip.action.canceled -= GripAction_canceled;
            controllerActionTrigger.action.canceled -= TriggerAction_canceled;
            controllerActionPrimary.action.performed -= PrimaryAction_canceled;
        }

        private void GripAction_performed(InputAction.CallbackContext obj) {
            DoFingerAnimation(grippingFingers, 1.0f);
        }

        private void TriggerAction_performed(InputAction.CallbackContext obj) {
            DoFingerAnimation(pointingFingers, 1.0f);
        }

        private void PrimaryAction_performed(InputAction.CallbackContext obj) {
            DoFingerAnimation(primaryFingers, 1.0f);
        }

        private void GripAction_canceled(InputAction.CallbackContext obj) {
            DoFingerAnimation(grippingFingers, 0.0f);
        }

        private void TriggerAction_canceled(InputAction.CallbackContext obj) {
            DoFingerAnimation(pointingFingers, 0.0f);
        }

        private void PrimaryAction_canceled(InputAction.CallbackContext obj) {
            DoFingerAnimation(primaryFingers, 0.0f);
        }

        //Déplace les doigts de fingersToAnimate vers targetValue (ouvert/fermé)
        private void DoFingerAnimation(List<Finger> fingersToAnimate, float targetValue) {
            foreach (Finger finger in fingersToAnimate) {
                finger.Target = targetValue;
            }

            foreach (Finger finger in fingersToAnimate) {
                string fingerName = finger.Type.ToString();
                float animationBlendValue = finger.Target;
                handAnimator.SetFloat(fingerName, animationBlendValue);
            }
        }
    }
}
