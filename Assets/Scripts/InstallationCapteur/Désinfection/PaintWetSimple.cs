using UnityEngine;

public class PaintWetSimple : MonoBehaviour
{
    [Header("Setup")]
    public Color colorSec = new Color(0.906f, 0.725f, 0.615f);  // #E7B99D
    public Color colorHumide = new Color(0.851f, 0.639f, 0.498f); // #D9A37F
    public float targetWetness = 1f;
    public float wetSpeed = 0.5f;

    private Renderer rend;
    private float wetness = 0f;
    private bool isComplete = false;

    public TaskManager taskManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = colorSec; // couleur initiale
        Debug.Log($"[PaintWet] Initialized on {gameObject.name}");
    }

    // Cette méthode sera appelée par LingetteNotifier
    public void AddWetness(float delta)
    {
        if (isComplete) return;

        // Accumulation progressive
        wetness = Mathf.Clamp01(wetness + delta * wetSpeed);

        // Interpolation progressive entre sec et humide
        rend.material.color = Color.Lerp(colorSec, colorHumide, wetness);

        Debug.Log($"[PaintWet] {gameObject.name} wetness: {wetness:F2}");

        if (wetness >= targetWetness)
        {
            taskManager?.NextStep();
            isComplete = true;
            Debug.Log($"[PaintWet] {gameObject.name} reached target wetness.");
        }
    }
}
