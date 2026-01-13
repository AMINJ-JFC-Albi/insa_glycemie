using System.Collections.Generic;
using HoloWatch;
using UnityEngine;

[System.Serializable]

public class SlotManager : MonoBehaviour
{
    public GameObject[] requiredObjects;
    public BigPoster bigPoster;

    public TaskManager taskManager;

    public void ValidateSlots()
{
    bool allFilled = true;

    foreach (var var in requiredObjects)
    {
        SnapToSlot snap = var.GetComponent<SnapToSlot>();
        if (snap == null || !snap.IsValid())
        {
            allFilled = false;
            break;
        }
    }

    if (allFilled)
    {
        Debug.Log("Tous les slots sont remplis ! BigPoster activable.");

        bigPoster.EnableGrab();

        taskManager.StartSteps();
        HolowatchUI.Instance.SetNextHints("Door", new List<string>() {"2-3", "2-4", "2-5", "2-6"});

    }
    else
    {
        Debug.Log("Pas complet");
    }
}

}
