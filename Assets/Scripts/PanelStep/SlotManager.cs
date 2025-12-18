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

    foreach (var slot in requiredObjects)
    {
        SnapToSlot snap = slot.GetComponent<SnapToSlot>();
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
