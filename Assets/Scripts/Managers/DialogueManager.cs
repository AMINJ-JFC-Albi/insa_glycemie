using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DialogueManager))]
public class DialogueSystemEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DialogueManager dialogueScript = (DialogueManager)target;

        if (GUILayout.Button("Increment Progression Step")) {
            dialogueScript.ShowNextDialogue();
        }

        if (GUILayout.Button("Reset Progression Step")) {
            dialogueScript.ResetStep();
            dialogueScript.ShowDialogue();
        }
    }
}
#endif

public class DialogueManager : MonoBehaviour {
    [SerializeField] private AudioClip[] listTTS;
    [SerializeField] private TextMeshProUGUI dialogueTMP;
    [SerializeField]
    private List<string> texts = new List<string>();

    private AudioSource audioSource;
    private int progressionStep = 0;
    private bool isSecondTextShow = false;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        if (dialogueTMP == null) throw new System.Exception("TextMeshProUGUI dialogueTMP null !");
        if (texts.Count == 0) throw new System.Exception("List<string> texts is empty !");
        ShowDialogue();
    }

    public void ResetStep() {
        progressionStep = 0;
    }

    private void IncrementStep() {
        progressionStep++;
        if (progressionStep >= texts.Count) progressionStep = texts.Count;
        if (progressionStep >= listTTS.Length) progressionStep = listTTS.Length;
    }

    public void ShowDialogue() {
        dialogueTMP.text = progressionStep >= texts.Count ? "" : ParseText(texts[progressionStep]);

        if (progressionStep < listTTS.Length) {
            HandleAudioPlayback();
        }
    }
    
    private void HandleAudioPlayback() {
        switch (progressionStep) {
            case 0:
                StartCoroutine(PlayAudioWithDelay(3f));
                break;
            case 1: {
                if (!isSecondTextShow) {
                    PlayAudio();
                    isSecondTextShow = true;
                }
                break;
            }
            default:
                PlayAudio();
                break;
        }
    }

    private void PlayAudio() {
        audioSource.clip = listTTS[progressionStep];
        audioSource.Play();
    }

    private IEnumerator PlayAudioWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        PlayAudio();
    }
    
    private string ParseText(string text) {
        StringBuilder sb = new StringBuilder();
        foreach (string line in text.Split("[endline]"))
            sb.AppendLine(line
                .Replace("[checklist]", GameManager.Instance.step1CheckList.ToString()));
        return sb.ToString();
    }

    public void ShowNextDialogue() {
        IncrementStep();
        ShowDialogue();
    }
}
