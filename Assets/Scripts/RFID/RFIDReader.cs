using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFID {
    public class RFIDReader : MonoBehaviour {
        private string badgeTag = "RFIDBadge";
        private float requiredTime = 2f;
        [SerializeField] private Material indicatorMaterial;
        
        private Dictionary<int, Coroutine> attempts = new Dictionary<int, Coroutine>();
        private bool done;
        
        private void Start() {
            indicatorMaterial.EnableKeyword("_EMISSION");
            indicatorMaterial.SetColor("_EmissionColor", Color.red);
        }

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag(badgeTag) || done) return;
            int id = other.gameObject.GetInstanceID();
            if (attempts.ContainsKey(id)) return;
            attempts[id] = StartCoroutine(HoldTimer(id));
        }

        void OnTriggerExit(Collider other) {
            if (!other.CompareTag(badgeTag) || done) return;
            indicatorMaterial.EnableKeyword("_EMISSION");
            indicatorMaterial.SetColor("_EmissionColor", Color.red);
            int id = other.gameObject.GetInstanceID();
            if (attempts.TryGetValue(id, out Coroutine c)) {
                StopCoroutine(c);
                attempts.Remove(id);
            }
        }

        private IEnumerator HoldTimer(int id) {
            float elapsed = 0f;
            float blinkInterval = 0.2f;
            float blinkTimer = 0f;
            bool emitOn = true;
            indicatorMaterial.SetColor("_EmissionColor", Color.yellow);
            
            while (elapsed < requiredTime) {
                elapsed += Time.deltaTime;
                blinkTimer += Time.deltaTime;

                if (indicatorMaterial && blinkTimer >= blinkInterval) {
                    blinkTimer = 0f;
                    emitOn = !emitOn;
                    if (emitOn) {
                        indicatorMaterial.EnableKeyword("_EMISSION");
                    } else {
                        indicatorMaterial.DisableKeyword("_EMISSION");
                    }
                }
                yield return null;
            }

            attempts.Remove(id);
            done = true;
            indicatorMaterial.EnableKeyword("_EMISSION");
            indicatorMaterial.SetColor("_EmissionColor", Color.green);

            Success();
        }

        private void Success() {
            TabletManager.Instance.GoToSecondStep();
        }
    }
}
