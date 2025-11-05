using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using PortalOLD;
using UnityEngine.XR;

namespace RevealScene {
    public class SceneLoader : MonoBehaviour {
        private bool justOneLoad;
        
        void Update() { //DEBUG!!! test à supprimer après
            InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            if (!leftHand.isValid || justOneLoad) return;
            if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed) && xPressed) {
                justOneLoad = true;
                OnButtonPressed();
            }
        }

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        public void OnButtonPressed() {
            StartCoroutine(LoadSceneCoroutine("TESTScene"));
        }
        
        private IEnumerator LoadSceneCoroutine(string sceneName) {
            // On charge la scène en additive
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // On attend que le chargement soit terminé
            while (asyncOp is { isDone: false })
                yield return null;

            // La scène est maintenant active, à voir quoi faire après
            PortalManagerOLD.Instance.Init();
        }

    }
}
