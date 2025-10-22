using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StepsUI))]
public class StepsUIEditor : Editor {
    private int stepIndex = 0;
    private int subStepIndex = 0;
    private bool checkStepIfAllSubStepsChecked = false;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        StepsUI stepsUIScript = (StepsUI)target;

        if (GUILayout.Button("Update Step UI")) {
            stepsUIScript.UpdateUI();
        }

        // Options pour tester CheckSubStep
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Test CheckSubStep", EditorStyles.boldLabel);
        stepIndex = EditorGUILayout.IntField("Step Index", stepIndex);
        subStepIndex = EditorGUILayout.IntField("SubStep Index", subStepIndex);
        checkStepIfAllSubStepsChecked = EditorGUILayout.Toggle("Check Step If All SubSteps Checked", checkStepIfAllSubStepsChecked);

        if (GUILayout.Button("Check SubStep")) {
            // Vérifie si les indices sont valides avant d'appeler CheckSubStep
            if (stepIndex >= 0 && stepIndex < stepsUIScript.DataCount) {
                stepsUIScript.CheckSubStep(stepIndex, subStepIndex, checkStepIfAllSubStepsChecked);
                stepsUIScript.UpdateUI(); // Met à jour l'UI pour refléter les changements
            } else {
                Debug.LogWarning("Step index out of range!");
            }
        }
    }
}
#endif

public class StepsUI : MonoBehaviour {
    [Serializable]
    protected class StepsData {
        public bool isCheck = false;
        public string text = string.Empty;
        public List<SubStepsData> subSteps = new List<SubStepsData>();

        public void CheckStep() {
            isCheck = true;
        }

        public void CheckSubStep(int i, bool checkStepIfAllSubStepsChecked = false) {
            if (i < subSteps.Count) {
                subSteps[i].isCheck = true;
            }

            if (checkStepIfAllSubStepsChecked && subSteps.Count > 0) {
                bool isAllSubStepsChecked = true;

                foreach (SubStepsData subStep in subSteps) {
                    if (!subStep.isCheck) {
                        isAllSubStepsChecked = false;
                    }
                }

                isCheck = isAllSubStepsChecked;
            }
        }
        public bool IsAllStepsChecked() {
            bool isAllSubStepsChecked = true;

            foreach (SubStepsData subStep in subSteps) {
                if (!subStep.isCheck) {
                    isAllSubStepsChecked = false;
                }
            }
            return isCheck && isAllSubStepsChecked;
        }
    }

    [Serializable]
    protected class SubStepsData {
        public bool isCheck = false;
        public string text = string.Empty;
    }

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabObjectif;
    [SerializeField] private List<StepsData> data;

    private void Awake() {
        if (content == null) throw new Exception("GameObject content null!");
        if (data == null || data.Count == 0) {
            data = new List<StepsData>();
        } else {
            UpdateUI();
        }
    }

    public void UpdateUI() {
        ClearUI();
        foreach (StepsData step in data) {
            GameObject newStepText = Instantiate(prefabObjectif, content.transform);
            ObjectifUI oUI = newStepText.GetComponent<ObjectifUI>();
            oUI.UpdateUI(step.isCheck, step.text);

            foreach (SubStepsData subStep in step.subSteps) {
                GameObject newSubStepText = Instantiate(prefabObjectif, content.transform);
                ObjectifUI subOUI = newSubStepText.GetComponent<ObjectifUI>();
                subOUI.UpdateUI(subStep.isCheck, subStep.text, true);
            }
        }
    }

    private void ClearUI() {
        foreach (Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    public int DataCount => data.Count;

    public void CheckSubStep(int stepIndex, int subStepIndex, bool checkStepIfAllSubStepsChecked) {
        if (stepIndex >= 0 && stepIndex < data.Count) {
            data[stepIndex].CheckSubStep(subStepIndex, checkStepIfAllSubStepsChecked);
        } else {
            Debug.LogWarning("Step index out of range!");
        }
    }

#endif
}
