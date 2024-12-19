using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(StepsUI))]
public class StepsUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StepsUI dialogueScript = (StepsUI)target;

        if (GUILayout.Button("Update Step UI"))
        {
            dialogueScript.UpdateUI();
        }
    }
}
#endif

public class StepsUI : MonoBehaviour
{
    [Serializable]
    protected class StepsData
    {
        public bool isCheck = false;
        public string text = string.Empty;
        public List<SubStepsData> subSteps = new List<SubStepsData>();
        public void CheckStep() { 
            isCheck = true;
        }
        public bool IsAllStepsChecked()
        {
            bool isAllSubStepsChecked = true;

            foreach (SubStepsData subStep in subSteps)
            {
                if (!subStep.isCheck)
                {
                    isAllSubStepsChecked = false;
                }
            }     
            return isCheck && isAllSubStepsChecked;
        }
    }

    [Serializable]
    protected class SubStepsData
    {
        public bool isCheck = false;
        public string text = string.Empty;
        public void CheckSubStep() { 
            isCheck = true;
        }

    }

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject prefabObjectif;
    [SerializeField] private List<StepsData> data;

    private void Awake()
    {
        if (content == null) throw new Exception("GameObject content null!");
        if (data == null || data.Count == 0 )
        {
            data = new List<StepsData>();

        }
        else {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        ClearUI();
        foreach (StepsData step in data)
        {
            GameObject newStepText = Instantiate(prefabObjectif, content.transform);
            ObjectifUI oUI = newStepText.GetComponent<ObjectifUI>();
            oUI.UpdateUI(step.isCheck, step.text);

            foreach (SubStepsData subStep in step.subSteps)
            {
                GameObject newSubStepText = Instantiate(prefabObjectif, content.transform);
                ObjectifUI subOUI = newSubStepText.GetComponent<ObjectifUI>();
                subOUI.UpdateUI(subStep.isCheck, subStep.text, true);
            }
        }
    }

    private void ClearUI()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}