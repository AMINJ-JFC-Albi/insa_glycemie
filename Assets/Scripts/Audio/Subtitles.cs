using System.Collections;
using TMPro;
using UnityEngine;

namespace Audio {
    public class Subtitles : MonoBehaviour {
        private TMP_Text masterText;
        private TMP_Text graphicText;

        public IEnumerator Play() {
            Text("Oui ? Fait voir... mmmh intéressant.");
            yield return new WaitForSeconds(1f);
            Text("Alors je le répète pour être clair : ce qui est mesuré dans le sang, c’est la glycémie capillaire,");
            yield return new WaitForSeconds(1f);
            Text("pas la glycémie interstitielle… la nuance change tout.");
            yield return new WaitForSeconds(1f);
            Text("*soupir* Si seulement ils avaient compris ça plus tôt...");
            yield return new WaitForSeconds(1f);
            Text("Bref. Passons.");
        }
        
        private void Text(string text) {
            masterText.text = text;
            graphicText.text = text;
        }
    }
}
