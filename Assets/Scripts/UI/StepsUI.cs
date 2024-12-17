using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
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
    }

    [Serializable]
    protected class SubStepsData
    {
        public bool isCheck = false;
        public string text = string.Empty;

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
        
        text.text = "";
        foreach (StepsData step in data)
        {
            
            if (text != null) {
                text.text = $"{text.text}{(step.isCheck ? "[X]" : "[]")} {step.text} {"\n"}";
                if (step.isCheck) {
                    text.color = Color.gray;
                } else {
                    text.color = Color.white;
                }
            }

            foreach (SubStepsData subStep in step.subSteps ) {

                if (text != null)
                {
                    text.text = $"{text.text} {"    "}{(subStep.isCheck ? "[X]" : "[]")} {subStep.text} {"\n"}";
                    if (subStep.isCheck)
                    {
                        text.color = Color.gray;
                    }
                    else
                    {
                        text.color = Color.white;
                    }
                }
            }
        }

    }
}