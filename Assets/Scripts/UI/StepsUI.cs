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

        if (GUILayout.Button("Show Step UI Debug"))
        {
            dialogueScript.ShowDebug();
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
        public void checkStep() { 
            isCheck = true;
        }
        public bool isAllStepsChecked()
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
        public void checkSubStep() { 
            isCheck = true;
        }

    }

    [SerializeField] private List<StepsData> data;
    [SerializeField] private TMP_Text text;    
    private void Awake()
    {
        if (data == null || data.Count == 0 )
        {
            data = new List<StepsData>();

        }
        else {
            UpdateUI();
        }
    }
    public void ShowDebug()
    {
        data.ForEach(step =>
        {
            Debug.Log($"[{(step.isCheck ? "X" : " ")}] {step.text}");
        });
    }
    public void UpdateUI()
    {
        GameObject content = GameObject.Find("Content");

        foreach (StepsData step in data)
        {
            TMP_Text newStepText = Instantiate(text, content.transform);
            newStepText.text = $"{(step.isCheck ? "[X]" : "[]")} {step.text}";
            newStepText.color = (step.isCheck) ? Color.gray : Color.white ;

            foreach (SubStepsData subStep in step.subSteps)
            {
                TMP_Text newSubStepText = Instantiate(text, content.transform);
                newSubStepText.text = $"{"    "}{(subStep.isCheck ? "[X]" : "[]")} {subStep.text}";
                newSubStepText.color = (step.isCheck) ? Color.gray : Color.white;
            }
        }

    }
}