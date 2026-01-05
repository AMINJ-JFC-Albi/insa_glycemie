using UnityEngine;
using UnityEngine.UI;

public class ProximityButtonHandler : MonoBehaviour
{
    public Button button;
    public Sprite finalSprite;       // Sprite à afficher après 3 secondes
    public Image image;              // L'image du bouton
    public Collider sensorCollider;  // Le trigger du capteur
    public Collider targetCollider;  // Le collider de l'objet qui doit déclencher
    public float requiredTime = 3f;
    public TaskManager taskManager;

    private bool processStarted = false;
    private bool isInSensor = false;
    private float timer = 0f;
    private bool stepCompleted = false;

    void Start()
    {
        button.onClick.AddListener(OnButtonPressed);
    }

    void OnButtonPressed()
    {
        // Désactive l'interactable pour que le bouton reste en état "pressed"
        button.interactable = false;

        // Change directement l'image du bouton pour le sprite pressed
        if (button.targetGraphic is Image img)
        {
            img.sprite = button.spriteState.pressedSprite;
        }

        processStarted = true;
        timer = 0f;
        stepCompleted = false;
    }

    void Update()
    {
        if (!processStarted || stepCompleted) return;

        if (isInSensor)
        {
            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                CompleteStep();
            }
        }
        else
        {
            timer = 0f; // reset si on sort de la zone
        }
    }

    void CompleteStep()
    {
        if (finalSprite != null && image != null)
        {
            image.sprite = finalSprite;
        }

        taskManager?.NextStep();
        stepCompleted = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!processStarted || stepCompleted) return;

        // Vérifie que le collider entrant est bien celui de l'objet attendu
        if (other == targetCollider)
        {
            isInSensor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!processStarted || stepCompleted) return;

        if (other == targetCollider)
        {
            isInSensor = false;
        }
    }
}
