using System.Collections.Generic;
using HoloWatch;
using UnityEngine;

namespace Cryptex {
    public class CryptexManager : MonoBehaviour {
        [SerializeField] private CryptexWheel[] wheels;
        [SerializeField] private string correctWord = "CAPILLAIRE";

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
            enabled = false;
            Debug.Log("Cryptex opened!");
            HolowatchUI.Instance.SetNextHints("Password", new List<string>() {"3-2"});
        }
    }
}
