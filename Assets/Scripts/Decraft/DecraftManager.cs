using System.Collections.Generic;
using HoloWatch;
using NavKeypad;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Decraft {
    public class DecraftManager : MonoBehaviour {
        [SerializeField] private ConfigurableJoint targetJoint;
        [SerializeField] private Keypad keypad;
        private int currentCount;

        public XRGrabInteractable key;

        void Start()
        {
            key.enabled = false;
        }
        
        public void Add() {
            currentCount++;
            if (currentCount >= 4) {
                targetJoint.zMotion = ConfigurableJointMotion.Limited;
                keypad.capteurAlreadyDo = true;
                HolowatchUI.Instance.SetNextHints("Badge", new List<string>() {"4-3"});
                key.enabled = true;
            }
        }
        
        public void Remove() {
            currentCount--;
        }
    }
}
