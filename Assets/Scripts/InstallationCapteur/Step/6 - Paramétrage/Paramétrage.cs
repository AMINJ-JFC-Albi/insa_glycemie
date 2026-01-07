using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Paramétrage : MonoBehaviour
{
    [Header("UI Elements")]
    public List<TMP_Dropdown> dropdowns;        // Liste des Dropdowns
    public Button checkButton;              // Bouton Vérifier
    public Button retourButton;             // Bouton Retour
    public GameObject panelLancement;       // Panel Lancement Analyse
    public GameObject panelGlycemie;        // Panel Affichage Glycémie
    public GameObject panelMauvaisParam;    // Panel Mauvais Paramétrage
    public GameObject panelOption;          // Panel Option

    public GameObject panelBluetooth;

    public TextMeshProUGUI valeur_glycémie;

    public TaskManager taskManager;

    [Header("Settings")]
    public List<int> correctAnswers;        // Liste des indices corrects pour chaque Dropdown

    public int valeur = 60;

    private void Start()
    {
        // Masquer tous les panels au départ
        
        panelGlycemie.SetActive(false);
        panelMauvaisParam.SetActive(false);
        panelOption.SetActive(false);
        panelLancement.SetActive(false);

        //Rajouter pour récupérer la valeur du GameManager

    }

    public void StartingOption()
    {
        panelOption.SetActive(true);
        panelBluetooth.SetActive(false);
        checkButton.onClick.AddListener(OnCheckButtonPressed);
        retourButton.onClick.AddListener(OnRetourButtonPressed);
        Debug.Log("OK");
    }

    private void OnCheckButtonPressed()
    {
        bool allCorrect = true;

        // Vérifie toutes les réponses
        for (int i = 0; i < dropdowns.Count; i++)
        {
            if (i >= correctAnswers.Count || dropdowns[i].value != correctAnswers[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            // Bonne réponse pour tous les Dropdowns
            StartCoroutine(ShowLancementThenGlycemie());
            
        }
        else
        {
            // Mauvaise réponse
            panelMauvaisParam.SetActive(true);
        }
    }

    private void OnRetourButtonPressed()
    {
        // Masquer Mauvais Paramétrage et afficher Option
        panelMauvaisParam.SetActive(false);
        panelOption.SetActive(true);
    }

    private IEnumerator ShowLancementThenGlycemie()
    {
        panelLancement.SetActive(true);       // Affiche Lancement Analyse
        yield return new WaitForSeconds(5f);  // Attend 5 secondes
        panelLancement.SetActive(false);      // Masque Lancement Analyse
        valeur_glycémie.text = valeur.ToString() + " mg/dl";
        panelGlycemie.SetActive(true);        // Affiche Glycémie
        taskManager?.NextStep();
    }
}
