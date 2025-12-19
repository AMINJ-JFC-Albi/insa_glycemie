using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[System.Serializable]
public class Step
{
    public string stepName;
    public List<GameObject> objectsToActivate;
}

public class TaskManager : MonoBehaviour
{
    public List<TaskQuad> taskQuads; // Tous les quads des tâches
    public List<Step> steps;          // Chaque étape avec ses objets
    private int currentStep;

    void Start()
    {
        // Désactiver tous les XRGrabInteractable dès le départ
        foreach (Step step in steps)
        {
            foreach (GameObject obj in step.objectsToActivate)
            {
                if (obj.TryGetComponent<XRGrabInteractable>(out var grab))
                {
                    grab.enabled = false;
                }
            }
        }
        
        // On n'appelle pas StartSteps ici : le SlotManager déclenchera le début des étapes
    }

    public void StartSteps()
    {
        currentStep = 0;

        UpdateStep();
    }

    

    public void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            UpdateStep();
        }
    }

    private void UpdateStep()
    {
        // Activer uniquement les objets de l'étape courante
        foreach (GameObject obj in steps[currentStep].objectsToActivate)
        {
            if (obj.TryGetComponent<XRGrabInteractable>(out var grab))
            {
                grab.enabled = true;
            }
            else
            {
                Debug.LogWarning($"L'objet {obj.name} n'a pas de XRGrabInteractable !");
            }
        }

        // Mettre à jour les quads
        for (int i = 0; i < taskQuads.Count; i++)
        {
            if (i < currentStep) taskQuads[i].SetState(TaskState.Completed);
            else if (i == currentStep) taskQuads[i].SetState(TaskState.InProgress);
            else taskQuads[i].SetState(TaskState.NotStarted);
        }
    }
}
