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
        public void checkStep() { isCheck = true; }
    }

    [Serializable]
    protected class SubStepsData
    {
        public bool isCheck = false;
        public string text = string.Empty;
        public void checkSubStep() { isCheck = true; }

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
            
                string hexCode ;
            if (text != null) {
                if (step.isCheck) {
                    hexCode = "#808080";
                } else {
                    hexCode = "#FFFFFF";

                }
                text.text = $"<color={hexCode}>{text.text}{(step.isCheck ? "[X]" : "[]")} {step.text} </color>\n";
            }

            foreach (SubStepsData subStep in step.subSteps ) {

                if (text != null)
                {
                    if (subStep.isCheck)
                    {
                        hexCode = "#808080";
                    }
                    else
                    {
                        hexCode = "#FFFFFF";

                    }
                    text.text = $"<color={hexCode}>{text.text} {"    "}{(subStep.isCheck ? "[X]" : "[]")} {subStep.text}</color>\n"; 
                }
            }
        }

    }
}