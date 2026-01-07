using System.Collections.Generic;
using TMPro;
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
    public XRJoystick joystick;

    [Header("UI Image cible")]
    public Image targetImage;

    [Header("Grille de sprites")]
    public List<SpriteRow> spriteGrid;

    [Header("UI Text")]
    public TextMeshProUGUI number_images;

    private int currentRow = 0;
    private int currentColumn = 0;

    void Start()
    {
        if (targetImage == null || targetImage.sprite == null) return;

        // Recherche de la position (row / column) du sprite actuel
        for (int r = 0; r < spriteGrid.Count; r++)
        {
            for (int c = 0; c < spriteGrid[r].rowSprites.Count; c++)
            {
                if (spriteGrid[r].rowSprites[c] == targetImage.sprite)
                {
                    currentRow = r;
                    currentColumn = c;
                    UpdateSprite();
                    return;
                }
            }
        }
    }

    void Update()
    {
        if (joystick == null || spriteGrid.Count == 0) return;

        Vector2 input = joystick.value;

        int columnCount = spriteGrid[0].rowSprites.Count;

        int newColumn = Mathf.FloorToInt(
            Mathf.InverseLerp(-1f, 1f, input.x) * columnCount);
        int newRow = Mathf.FloorToInt(
            Mathf.InverseLerp(1f, -1f, input.y) * spriteGrid.Count);

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
        {
            targetImage.sprite = spriteGrid[currentRow].rowSprites[currentColumn];
        }

        // Mise à jour du texte (index humain)
        if (number_images != null)
        {
            int columnCount = spriteGrid[0].rowSprites.Count;
            int imageIndex = currentRow * columnCount + currentColumn + 1;
            number_images.text = imageIndex.ToString();
        }
    }
}
