using System.Collections.Generic;
using HoloWatch;
using NavKeypad;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Cryptex {
    public class CryptexManager : MonoBehaviour {
        [SerializeField] private CryptexWheel[] wheels;
        [SerializeField] private GameObject retainer;
        [SerializeField] private Keypad keypad;
        private string correctWord = "CAPILLAIRE";
        
        private void Update() {
            if (IsCorrect()) {
                OpenCryptex();
            }
        }

        private bool IsCorrect() {
            if (wheels.Length != correctWord.Length) {
                return false;
            }

            for (int i = 0; i < wheels.Length; i++) {
                if (wheels[i].GetCurrentLetter() != correctWord[i]) {
                    return false;
                }
            }
            
            return true;
        }

        private void OpenCryptex() {
            XRGrabInteractable retainerGrabInteractable = retainer.GetComponent<XRGrabInteractable>();
            XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
            
            enabled = false;
            retainer.transform.SetParent(null);
            
            Destroy(retainer.GetComponent<Collider>());
            MeshCollider meshCollider = retainer.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            
            retainer.GetComponent<Rigidbody>().isKinematic = false;
            retainerGrabInteractable.enabled = true;
            foreach (CryptexWheel wheel in wheels) {
                wheel.knob.enabled = false;
                Destroy(wheel.GetComponent<Collider>());
                meshCollider = wheel.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                grabInteractable.colliders.Add(meshCollider);
            }
            HolowatchUI.Instance.SetNextHints("Password", new List<string>() {"3-3"});
            keypad.cryptexAlreadyOpen = true;
        }
    }
}
