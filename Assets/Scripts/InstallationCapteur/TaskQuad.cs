using UnityEngine;

public class TaskQuad : MonoBehaviour
{
    public TaskState state;
    public Renderer quadRenderer;
    public Color unplacedColor = new(1f, 1f, 1f, 0f);
    public Color notStartedColor = Color.gray;
    public Color inProgressColor = Color.yellow;
    public Color completedColor = Color.green;

    void Start()
    {
        state = TaskState.Unplaced;
        UpdateBorder();
    }

    public void SetState(TaskState newState)
    {
        state = newState;
        UpdateBorder();
    }

    private void UpdateBorder()
    {
       switch (state)
    {
        case TaskState.Unplaced:    quadRenderer.material.color = unplacedColor; break;
        case TaskState.NotStarted:  quadRenderer.material.color = notStartedColor; break;
        case TaskState.InProgress:  quadRenderer.material.color = inProgressColor; break;
        case TaskState.Completed:   quadRenderer.material.color = completedColor; break;
    }
    }
}
