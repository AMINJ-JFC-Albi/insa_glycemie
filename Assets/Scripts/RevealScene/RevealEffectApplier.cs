using Unity.VisualScripting;
using UnityEngine;

namespace RevealScene {
    public class RevealEffectAllObjects : MonoBehaviour {
        public Shader revealShader;
        private Renderer[] allRenderers;
        private Material[] revealMaterials;
        
        public float waveSpeed = 5f;
        private float waveRadius = 0f;
        
        private static readonly int Color = Shader.PropertyToID("_BaseColor");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int WaveCenter = Shader.PropertyToID("_WaveCenter");
        private static readonly int WaveRadius = Shader.PropertyToID("_WaveRadius");
        
        private static readonly int ColorID = Shader.PropertyToID("_Color");
        private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
        private static readonly int AlbedoID = Shader.PropertyToID("_Albedo");
        private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
        private static readonly int EmissionMapID = Shader.PropertyToID("_EmissionMap");
        private static readonly int HaveEmission = Shader.PropertyToID("_HaveEmission");

        void Start() {
            // Trouve tous les Renderers dans la scène
            allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            revealMaterials = new Material[allRenderers.Length];

            for (int i = 0; i < allRenderers.Length; i++) {
                Material originalMat = allRenderers[i].material;
                Material revealMat = new Material(revealShader);

                // Copie la couleur et la texture du matériau original si possible
                if (originalMat.HasProperty(Color))
                    revealMat.SetColor(Color, originalMat.GetColor(Color));
                if (originalMat.HasProperty(MainTex)) //DEBUG!!! Pas utilisé ? Mais je ne vois pas de bug pour le moment
                    revealMat.SetTexture(MainTex, originalMat.GetTexture(MainTex));
                
                    
                if (originalMat.HasProperty(ColorID))
                    revealMat.SetColor(ColorID, originalMat.GetColor(ColorID));
                    
                if (originalMat.HasProperty(AlbedoID))
                    revealMat.SetColor(AlbedoID, originalMat.GetColor(AlbedoID));
                    
                if (originalMat.HasProperty(BaseMapID)) {
                    revealMat.SetTexture(BaseMapID, originalMat.GetTexture(BaseMapID));
                }
                
                if (originalMat.HasProperty(MainTexID)) {
                    revealMat.SetTexture(MainTexID, originalMat.GetTexture(MainTexID));
                }
                
                if (originalMat.HasProperty(EmissionMapID)) {
                    revealMat.SetTexture(EmissionMapID, originalMat.GetTexture(EmissionMapID));
                    revealMat.SetFloat(HaveEmission, originalMat.IsKeywordEnabled("_EMISSION") ? 1 : 0);
                }

                // Assigne le matériau "reveal"
                allRenderers[i].material = revealMat;
                revealMaterials[i] = revealMat;
            }
        }
        
        void Update() {
            waveRadius += waveSpeed * Time.deltaTime;

            for (int i = 0; i < allRenderers.Length; i++) {
                if(revealMaterials[i].HasProperty(WaveCenter))
                    revealMaterials[i].SetVector(WaveCenter, transform.position);
                if(revealMaterials[i].HasProperty(WaveRadius))
                    revealMaterials[i].SetFloat(WaveRadius, waveRadius);
            }
        }
    }
}
