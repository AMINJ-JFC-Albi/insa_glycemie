using UnityEngine;

namespace Audio {
    [RequireComponent(typeof(AudioSource))]
    public class AudioTransmitter : MonoBehaviour {
        private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        
        private void Awake() {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play() {
            StartCoroutine(GetComponentInChildren<Subtitles>().Play());
            if (audioSource != null && audioClip != null) {
                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}
