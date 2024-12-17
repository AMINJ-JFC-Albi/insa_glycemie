using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(QuestionSystem))]
public class QuestionSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QuestionSystem questionScript = (QuestionSystem)target;

        if (GUILayout.Button("Increment Progression Step"))
        {
            questionScript.ShowNextQuestion();
        }

        if (GUILayout.Button("Reset Progression Step"))
        {
            questionScript.ResetStep();
            questionScript.ShowQuestion();
        }
    }
}
#endif

public class QuestionSystem : MonoBehaviour
{
    [Serializable]
    public class Question
    {
        [Serializable]
        public enum GoodAnswer
        {
            A, B, C
        }
        [SerializeField] public string question;
        [SerializeField] public string answerA;
        [SerializeField] public string answerB;
        [SerializeField] public string answerC;
        [SerializeField] public GoodAnswer goodAnswer;
    }

    [Header("Question")]
    [SerializeField] private TextMeshProUGUI questTMP;
    
    [Header("Answer A")]
    [SerializeField] private TextMeshProUGUI answATMP;
    [Header("Answer B")]
    [SerializeField] private TextMeshProUGUI answBTMP;
    [Header("Answer C")]
    [SerializeField] private TextMeshProUGUI answCTMP;

    [Header("Datas")]
    [SerializeField] private List<Question> datas;

    private int progressionStep = 0;

    void Start()
    {
        if (questTMP == null) throw new System.Exception("TextMeshProUGUI questTMP null !");
        if (datas.Count == 0) throw new System.Exception("List<Question> datas is empty !");
        ShowQuestion();
    }

    public void ResetStep()
    {
        progressionStep = 0;
    }

    public void IncrementStep()
    {
        progressionStep++;
        if (progressionStep >= datas.Count) progressionStep = datas.Count;
    }

    public void ShowQuestion()
    {
        if (progressionStep >= datas.Count)
        {
            questTMP.text = "";
            answATMP.text = "";
            answBTMP.text = "";
            answCTMP.text = "";
        }
        else
        {
            questTMP.text = datas[progressionStep].question;
            answATMP.text = datas[progressionStep].answerA;
            answBTMP.text = datas[progressionStep].answerB;
            answCTMP.text = datas[progressionStep].answerC;
        }
    }

    public void ShowNextQuestion()
    {
        IncrementStep();
        ShowQuestion();
    }
}