using UnityEngine;

[System.Serializable]

public class SlotManager : MonoBehaviour
{
    public SnapToSlot[] requiredObjects;
    public BigPoster bigPoster;

    public TaskManager taskManager;

    public void ValidateSlots()
{
    bool allFilled = true;

    foreach (var snap in requiredObjects)
    {
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

    }
    else
    {
        Debug.Log("Pas complet");
    }
}

}
