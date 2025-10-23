using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectifUI : MonoBehaviour {
    [SerializeField] private GameObject checkedGO;
    [SerializeField] private Image checkedGOImage;
    [SerializeField] private RectTransform checkedRect;
    [SerializeField] private GameObject uncheckedGO;
    [SerializeField] private Image uncheckedGOImage;
    [SerializeField] private RectTransform uncheckedRect;
    [SerializeField] private TextMeshProUGUI objectifTMP;
    [SerializeField] private RectTransform textRect;

    public void UpdateUI(bool isCheck, string text, bool isSubObjectif = false) {
        UpdateUI(isCheck, text, isCheck ? Color.green : Color.white, isSubObjectif);
    }

    public void UpdateUI(bool isCheck, string text, Color color, bool isSubObjectif = false) {
        // Mise à jour du texte
        objectifTMP.text = text;

        // Mise à jour des états des checkboxes
        checkedGO.SetActive(isCheck);
        uncheckedGO.SetActive(!isCheck);

        // Mise à jour de la couleur des images
        checkedGOImage.color = color;
        uncheckedGOImage.color = color;

        // Mise à jour des positions pour les sous-objectifs
        float checkedX = isSubObjectif ? -950f : -1050f;
        float textX = isSubObjectif ? 300f : 200f;

        // Modification des positions des objets
        checkedRect.anchoredPosition = new Vector2(checkedX, checkedRect.anchoredPosition.y);
        uncheckedRect.anchoredPosition = new Vector2(checkedX, uncheckedRect.anchoredPosition.y);
        textRect.anchoredPosition = new Vector2(textX, textRect.anchoredPosition.y);
    }
}
