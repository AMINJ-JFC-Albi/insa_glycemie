using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

namespace RevealScene {
    public class SceneLoader : MonoBehaviour {
        private static readonly int WaveCenterID = Shader.PropertyToID("_WaveCenter");
        private static readonly int WaveRadiusID = Shader.PropertyToID("_WaveRadius");
        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
        private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
        private static readonly int EmissionMapID = Shader.PropertyToID("_EmissionMap");
        private static readonly int HaveEmission = Shader.PropertyToID("_HaveEmission");
        
        [SerializeField] private Shader revealShader;

        [SerializeField] private float revealDuration = 10f;
        [SerializeField] private float waveSpeed = 3f;
        [SerializeField] private float startRadius = 0f;
        

        void Update() { //DEBUG!!! test à supprimer après
            InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

            if (!leftHand.isValid) return;
            if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed) && xPressed) {
                OnButtonPressed();
            }
        }

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        public void OnButtonPressed() {
            StartCoroutine(LoadAndRevealSceneCoroutine("LevelScene"));
        }

        private IEnumerator LoadAndRevealSceneCoroutine(string sceneName) {
            // On charge la scène en additive
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            while (asyncOp is { isDone: false }) // On attend qu'elle soit correctement chargé
                yield return null;

            // On récupère tous les matériaux de la nouvelle scène et on leur applique le shader graph DEBUG!!! Attendre un peu avant de lancer l'animation ?
            List<Renderer> renderers = new List<Renderer>();
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects()) {
                foreach (Renderer rend in go.GetComponentsInChildren<Renderer>(true)) {
                    Material originalMat = rend.material;
                    Material revealMat = new Material(revealShader);

                    if (rend.material.HasProperty(BaseColorID))
                        revealMat.SetColor(BaseColorID, rend.material.GetColor(BaseColorID));
                    
                    if (rend.material.HasProperty(BaseMapID)) {
                        revealMat.SetTexture(BaseMapID, rend.material.GetTexture(BaseMapID));
                    }
                    
                    if (originalMat.HasProperty(EmissionMapID)) {
                        revealMat.SetTexture(EmissionMapID, originalMat.GetTexture(EmissionMapID));
                        revealMat.SetFloat(HaveEmission, originalMat.IsKeywordEnabled("_EMISSION") ? 1 : 0);
                    }

                    rend.material = revealMat;
                    renderers.Add(rend);
                }
            }

            // On commence l'animation de révélation
            Vector3 center = FindFirstObjectByType<XROrigin>().transform.position;
            float waveRadius = startRadius;
            float elapsed = 0f;
            while (waveRadius < 1000f) {
                waveRadius += waveSpeed * Time.deltaTime;
                elapsed += Time.deltaTime;

                foreach (Material mat in renderers.Select(rend => rend.material)) {
                    if (mat.HasProperty(WaveCenterID))
                        mat.SetVector(WaveCenterID, center);
                    if (mat.HasProperty(WaveRadiusID))
                        mat.SetFloat(WaveRadiusID, waveRadius);
                }

                yield return null;
                // On arrête la vague après un certain temps
                if (elapsed > revealDuration)
                    break;
            }
        }
    }
}
