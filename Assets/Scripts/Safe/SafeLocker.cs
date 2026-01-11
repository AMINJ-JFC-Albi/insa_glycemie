using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Safe {
    public class SafeLocker : MonoBehaviour {
        [SerializeField] private Material led;
        
        private XRKnob knob;
        private XRGrabInteractable grab;

        public XRGrabInteractable badge;
        
        private void Start() {
            knob = GetComponentInChildren<XRKnob>();
            knob.onValueChange.AddListener(OnKnobTurned);
            knob.clampedMotion = true;
            
            grab = GetComponentInChildren<XRGrabInteractable>();
            
            led.SetColor("_EmissionColor", Color.red);
            led.color = Color.red;

            badge.enabled = false;
        }
        
        public void CanOpenSafe() {
            knob.clampedMotion = false;
            led.SetColor("_EmissionColor", Color.green);
            led.color = Color.green;
            badge.enabled = true;
        }

        private void OnKnobTurned(float value) {
            if (Mathf.Abs(value) >= 10) {
                knob.onValueChange.RemoveListener(OnKnobTurned);
                knob.enabled = false;
                grab.enabled = true;
            }
        }
    }
}
