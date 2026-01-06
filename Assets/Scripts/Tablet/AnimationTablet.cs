using System.Collections;
using TMPro;
using UnityEngine;

namespace Tablet {
    public class AnimationTablet : MonoBehaviour {
        private TMP_Text textComponent;

        private float dotInterval = 1f;
        private int maxDots = 3;

        private string baseText;
        private Coroutine dotsCoroutine;

        private void Start() {
            textComponent = GetComponent<TMP_Text>();
            baseText = textComponent.text;
            dotsCoroutine ??= StartCoroutine(AnimateDots());
        }

        private IEnumerator AnimateDots() {
            int count = 0;
            while (true) {
                count = (count + 1) % (maxDots + 1);
                textComponent.text = baseText + new string('.', count);
                yield return new WaitForSeconds(dotInterval);
            }
        }
        
        private void OnEnable() {
            if (textComponent) {
                dotsCoroutine ??= StartCoroutine(AnimateDots());
            }
        }

        private void OnDisable() {
            if (dotsCoroutine != null) {
                StopCoroutine(dotsCoroutine);
                dotsCoroutine = null;
            }

            if (textComponent != null)
                textComponent.text = baseText;
        }
    }
}
