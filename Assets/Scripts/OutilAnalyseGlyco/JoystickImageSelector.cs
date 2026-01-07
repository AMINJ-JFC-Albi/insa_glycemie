using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class SpriteRow
{
    public List<Sprite> rowSprites;
}

public class JoystickImageSelector : MonoBehaviour
{
    [Header("Joystick XR")]
    public XRJoystick joystick; // ton XR Joystick

    [Header("UI Image cible")]
    public Image targetImage;

    [Header("Grille de sprites")]
    public List<SpriteRow> spriteGrid;

    private int currentRow = 0;
    private int currentColumn = 0;

    void Update()
    {
        if (joystick == null || spriteGrid.Count == 0) return;

        Vector2 input = joystick.value; // X = horizontal, Y = vertical (-1 à 1)
        
        // Convertir X/Y en indices
        int newColumn = Mathf.FloorToInt(Mathf.InverseLerp(-1f, 1f, input.x) * spriteGrid[0].rowSprites.Count);
        int newRow    = Mathf.FloorToInt(Mathf.InverseLerp(-1f, 1f, input.y) * spriteGrid.Count);

        newColumn = Mathf.Clamp(newColumn, 0, spriteGrid[0].rowSprites.Count - 1);
        newRow    = Mathf.Clamp(newRow, 0, spriteGrid.Count - 1);

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
        {
            targetImage.sprite = spriteGrid[currentRow].rowSprites[currentColumn];
        }
    }
}
