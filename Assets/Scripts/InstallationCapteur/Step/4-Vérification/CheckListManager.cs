using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChecklistManager : MonoBehaviour
{
    [Header("Toggles de la checklist")]
    public List<Toggle> toggles;

    [Header("Référence au TaskManager")]
    public TaskManager taskManager;

    private bool preventUncheck = false;

    void Start()
    {
        // Ajouter les listeners à tous les toggles
        foreach (Toggle t in toggles)
        {
            t.onValueChanged.AddListener((isOn) => OnToggleChanged(t, isOn));
        }
    }

    void OnToggleChanged(Toggle changedToggle, bool isOn)
    {
        if (preventUncheck && !isOn)
        {
            // Empêche de décocher après validation
            changedToggle.isOn = true;
            return;
        }

        // Vérifie si tous les toggles sont cochés
        bool allChecked = true;
        foreach (Toggle t in toggles)
        {
            if (!t.isOn)
            {
                allChecked = false;
                break;
            }
        }

        if (allChecked)
        {
            Debug.Log("Toutes les tâches sont cochées !");
            
            // Appelle NextStep si taskManager est assigné
            taskManager?.NextStep();

            // Bloque les décochages
            preventUncheck = true;

            // Optionnel : désactiver tous les toggles pour l'utilisateur
            foreach (Toggle t in toggles)
            {
                t.interactable = false;
            }
        }
    }
}
