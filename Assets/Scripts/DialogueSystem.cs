using UnityEngine;
using TMPro;
using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI dialogueTMP;
    [SerializeField] private List<string> texts = new()
    {
        "Bienvenue dans X. Pour continuer, nous devons d'abord reconstruire ton capteur de glycémie...",
        "Je vais te guider pour assembler ce capteur. Suis la ligne lumineuse jusqu'au premier composant.",
        "Pour ramasser cet objet, appuie sur [Touche Grab].",
        "Excellent ! Maintenant, dépose l'objet sur la table de craft pour commencer l'assemblage.",
        "Il te reste encore [X] composants à récupérer dans la salle. Suis les lignes lumineuses pour les retrouver."
    };
    private int progressionStep = 0;

    void Start()
    {
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
    }

    public void ShowDialogue()
    {
        if (progressionStep >= texts.Count)
            dialogueTMP.text = "";
        else
            dialogueTMP.text = texts[progressionStep];
    }

    public void ShowNextDialogue()
    {
        IncrementStep();
        ShowDialogue();
    }
}