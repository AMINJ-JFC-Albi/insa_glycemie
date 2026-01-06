using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.SpatialKeyboard;

public class TabletManager : MonoBehaviour {
    public static TabletManager Instance;
    
    [SerializeField] private GameObject firstStep;
    [SerializeField] private GameObject secondStep;
    [SerializeField] private TMP_Text textSecondStep;
    [SerializeField] private TMP_Text textBluetooth;
    [SerializeField] private XRKeyboard keyboard;
    
    [SerializeField] private GameObject thirdStep;
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    public void Start() {
        firstStep.SetActive(true);
        secondStep.SetActive(false);
        //thirdStep.SetActive(false);
    }
    
    public void GoToSecondStep() {
        keyboard.Clear();
        textBluetooth.text = "Bluetooth: Connected";
        firstStep.SetActive(false);
        secondStep.SetActive(true);
    }
    
    public void GoToThirdStep() {
        string cleaned = Regex.Replace(textSecondStep.text ?? "", @"[\uFEFF\u00A0\u200B-\u200D]", "");
        cleaned = cleaned.Normalize(NormalizationForm.FormC);
        if (secondStep.activeSelf && string.Equals(cleaned.Trim(), "care", System.StringComparison.OrdinalIgnoreCase)) {
            secondStep.SetActive(false);
            //thirdStep.SetActive(true);
        }
    }
}
