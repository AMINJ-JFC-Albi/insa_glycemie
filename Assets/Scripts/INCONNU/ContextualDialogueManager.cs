using UnityEngine;
using TMPro;

public class ContextualDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private int progressionStep = 0; // Étape de progression

    void Start()
    {
        StartIntroduction(); // Lance l'introduction au début de la scène
    }

    public void StartIntroduction()
    {
        dialogueText.text = "Bienvenue dans X. Pour continuer, nous devons d'abord reconstruire ton capteur de glycémie...";
        progressionStep = 1; // Passe à l'étape 1
    }

    public void ShowNextDialogue()
    {
        switch (progressionStep)
        {
            case 1:
                dialogueText.text = "Je vais te guider pour assembler ce capteur. Suis la ligne lumineuse jusqu'au premier composant.";
                progressionStep = 2;
                break;
            case 2:
                dialogueText.text = "Pour ramasser cet objet, appuie sur [Touche Grab].";
                progressionStep = 3;
                break;
            case 3:
                dialogueText.text = "Excellent ! Maintenant, dépose l'objet sur la table de craft pour commencer l'assemblage.";
                progressionStep = 4;
                break;
            case 4:
                dialogueText.text = "Il te reste encore [X] composants à récupérer dans la salle. Suis les lignes lumineuses pour les retrouver.";
                break;
            default:
                dialogueText.text = "";
                break;
        }
    }
}