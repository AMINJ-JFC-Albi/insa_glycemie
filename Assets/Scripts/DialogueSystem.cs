using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DialogueSystem))]
public class DialogueSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueSystem dialogueScript = (DialogueSystem)target;

        if (GUILayout.Button("Increment Progression Step"))
        {
            dialogueScript.ShowNextDialogue();
        }

        if (GUILayout.Button("Reset Progression Step"))
        {
            dialogueScript.ResetStep();
            dialogueScript.ShowDialogue();
        }
    }
}
#endif

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] listTTS;
    [SerializeField] private TextMeshProUGUI dialogueTMP;
    [SerializeField] private List<string> texts = new()
    {
        "Bienvenue dans X. Pour continuer, nous devons d'abord reconstruire ton capteur de glycémie...",
        "Je vais te guider pour assembler ce capteur. Suis la ligne lumineuse jusqu'au premier composant.",
        "Pour ramasser cet objet, appuie sur [Touche Grab].",
        "Excellent ! Maintenant, dépose l'objet sur la table de craft pour commencer l'assemblage.",
        "Il te reste encore [X] composants à récupérer dans la salle. Suis les lignes lumineuses pour les retrouver."
    };

    private AudioSource audioSource;
    private int progressionStep = 0;
    private bool isSecondTextShow = false; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (dialogueTMP == null) throw new System.Exception("TextMeshProUGUI dialogueTMP null !");
        if (texts.Count == 0) throw new System.Exception("List<string> texts is empty !");
        ShowDialogue();
    }

    public void ResetStep()
    {
        progressionStep = 0;
    }

    public void IncrementStep()
    {
        progressionStep++;
        if (progressionStep >= texts.Count) progressionStep = texts.Count;
        if (progressionStep >= listTTS.Length) progressionStep = listTTS.Length;
    }

    public void ShowDialogue()
    {
        if (progressionStep >= texts.Count)
        {
            dialogueTMP.text = "";
        }
        else
        {
            dialogueTMP.text = ParseText(texts[progressionStep]);
        }

        if (progressionStep < listTTS.Length)
        {
            HandleAudioPlayback();
        }
    }
    private void HandleAudioPlayback()
    {
        if (progressionStep == 0)
        {
            StartCoroutine(PlayAudioWithDelay(3f));
        }
        else if (progressionStep == 1)
        {
            if (!isSecondTextShow)
            {
                PlayAudio();
                isSecondTextShow = true;
            }
        }
        else
        {
            PlayAudio();
        }
    }

    private void PlayAudio()
    {
        audioSource.clip = listTTS[progressionStep];
        audioSource.Play();
    }

    private IEnumerator PlayAudioWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAudio();
    }


    public string ParseText(string text)
    {
        StringBuilder sb = new();
        foreach(string line in text.Split("[endline]")) sb.AppendLine(line
            .Replace("[checklist]", GameManager.Instance.step1CheckList.ToString()));
        return sb.ToString();
    }

    public void ShowNextDialogue()
    {
        IncrementStep();
        ShowDialogue();
    }
}