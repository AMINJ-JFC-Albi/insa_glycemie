using System.Collections;
using TMPro;
using UnityEngine;

namespace Audio {
    public class Subtitles : MonoBehaviour {
        private TMP_Text masterText;
        private TMP_Text graphicText;
        private Canvas canvas;
        
        private void Start() {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }

        public IEnumerator Play() {
            if (canvas.enabled) yield break;
            
            Text("Oui ? Fait voir... mmmh intéressant.");
            canvas.enabled = true;
            yield return new WaitForSeconds(4f);
            Text("Alors je le répète pour être clair : ce qui est mesuré dans le sang, c’est la glycémie CAPILLAIRE,");
            yield return new WaitForSeconds(10f);
            Text("pas la glycémie interstitielle. La nuance change tout.");
            yield return new WaitForSeconds(5f);
            Text("*soupir* Si seulement ils avaient compris ça plus tôt...");
            yield return new WaitForSeconds(5f);
            Text("Bref. Passons.");
            yield return new WaitForSeconds(3f);
            canvas.enabled = false;
        }
        
        private void Text(string text) {
            masterText.text = text;
            graphicText.text = text;
        }
    }
}
