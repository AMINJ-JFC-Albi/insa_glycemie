using System.Collections.Generic;
using HoloWatch;
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
            HolowatchUI.Instance.SetNextHints("Badge", new List<string>() { "4-5" });
        }

        private void OnKnobTurned(float value) {
            if (Mathf.Abs(value) >= 90) {
                knob.onValueChange.RemoveListener(OnKnobTurned);
                knob.enabled = false;
                grab.enabled = true;
                badge.enabled = true;
            }
        }
    }
}
