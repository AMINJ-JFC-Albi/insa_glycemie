using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[System.Serializable]
public class SpriteRow
{
    public List<Sprite> rowSprites;
}

public class JoystickImageSelector : MonoBehaviour
{
    [Header("Joystick XR")]
    public XRJoystick joystick;

    [Header("UI Image cible")]
    public Image targetImage;

    [Header("Grille de sprites")]
    public List<SpriteRow> spriteGrid;

    [Header("Sprite Explication")]
    public Sprite image_explication;

    [Header("UI Text index image")]
    public TextMeshProUGUI number_images;

    [Header("UI Message résultat")]
    public TextMeshProUGUI resultText;

    [Header("Objets XR à manipuler")]
    public GameObject joystickObject;
    public GameObject buttonObject;

    [Header("XR Grip Button")]
    public XRGripButton gripButton;

    private int currentRow = 0;
    private int currentColumn = 0;
    private bool calibrationDone = false;
    private bool waitingForRetry = false;

    void Start()
    {
        if (targetImage == null || spriteGrid.Count == 0) return;

        // Trouver la position correspondant au sprite déjà affiché
        for (int r = 0; r < spriteGrid.Count; r++)
        {
            for (int c = 0; c < spriteGrid[r].rowSprites.Count; c++)
            {
                if (spriteGrid[r].rowSprites[c] == targetImage.sprite)
                {
                    currentRow = r;
                    currentColumn = c;
                    UpdateSprite();
                    break;
                }
            }
        }

        // 🔥 Synchronisation IMAGE → JOYSTICK au démarrage
        SetJoystickFromImage();

        if (gripButton != null)
            gripButton.onPress.AddListener(OnGripButtonPressed);
    }

    void Update()
    {
        if (calibrationDone || waitingForRetry) return;
        if (joystick == null || spriteGrid.Count == 0) return;

        Vector2 input = joystick.value;

        int columnCount = spriteGrid[0].rowSprites.Count;

        int newColumn = Mathf.FloorToInt(Mathf.InverseLerp(-1f, 1f, input.x) * columnCount);
        int newRow = Mathf.FloorToInt(Mathf.InverseLerp(1f, -1f, input.y) * spriteGrid.Count);

        newColumn = Mathf.Clamp(newColumn, 0, columnCount - 1);
        newRow = Mathf.Clamp(newRow, 0, spriteGrid.Count - 1);

        if (newRow != currentRow || newColumn != currentColumn)
        {
            currentRow = newRow;
            currentColumn = newColumn;
            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        if (targetImage != null)
            targetImage.sprite = spriteGrid[currentRow].rowSprites[currentColumn];

        if (number_images != null)
        {
            int columnCount = spriteGrid[0].rowSprites.Count;
            int imageIndex = currentRow * columnCount + currentColumn + 1;
            number_images.text = imageIndex.ToString();
        }
    }

    // 🔁 Conversion IMAGE → JOYSTICK (utilisé une seule fois au Start)
    void SetJoystickFromImage()
    {
        if (joystick == null) return;

        int columnCount = spriteGrid[0].rowSprites.Count;
        int rowCount = spriteGrid.Count;

        float x = Mathf.Lerp(-1f, 1f, currentColumn / (float)(columnCount - 1));
        float y = Mathf.Lerp(1f, -1f, currentRow / (float)(rowCount - 1));

        joystick.value = new Vector2(x, y);
    }

    private void OnGripButtonPressed()
    {
        if (calibrationDone) return;

        if (waitingForRetry)
        {
            if (resultText != null)
                resultText.text = "Re-calibrage des données de simulation\nen mode manuel obligatoire";

            if (joystick != null)
                joystick.enabled = true;

            waitingForRetry = false;
            return;
        }

        ValidateCalibration();
    }

    private void ValidateCalibration()
    {
        bool goodImage = (currentRow == 1 && currentColumn == 1);

        if (goodImage)
        {
            if (resultText != null)
                resultText.text = "Re-calibrage des données réussi !";

            targetImage.sprite = image_explication;

            DisableInteractions();
            calibrationDone = true;
        }
        else
        {
            if (resultText != null)
                resultText.text = "Mauvais calibrage. Appuyer sur le bouton pour recommencer";

            if (joystick != null)
                joystick.enabled = false;

            waitingForRetry = true;
        }
    }

    private void DisableInteractions()
    {
        if (joystickObject != null)
        {
            XRJoystick joystickComp = joystickObject.GetComponent<XRJoystick>();
            if (joystickComp != null)
                joystickComp.enabled = false;

            Rigidbody rb = joystickObject.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;

            XRGrabInteractable grab = joystickObject.GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = false;
        }

        if (buttonObject != null)
        {
            XRGrabInteractable grabComp = buttonObject.GetComponent<XRGrabInteractable>();
            if (grabComp != null)
                grabComp.enabled = false;

            XRBaseInteractable baseComp = buttonObject.GetComponent<XRBaseInteractable>();
            if (baseComp != null)
                baseComp.enabled = false;

            Rigidbody rb = buttonObject.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
        }

        this.enabled = false;
    }

    void OnDestroy()
    {
        if (gripButton != null)
            gripButton.onPress.RemoveListener(OnGripButtonPressed);
    }
}
