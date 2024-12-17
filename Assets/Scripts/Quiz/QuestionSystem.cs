using UnityEngine;
using TMPro;
using System.Collections.Generic;
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
    [Header("Question")]
    [SerializeField] private TextMeshProUGUI questTMP;
    [SerializeField] private List<string> questions;
    
    [Header("Answers A")]
    [SerializeField] private TextMeshProUGUI answATMP;
    [SerializeField] private List<string> answersA;
    [Header("Answers B")]
    [SerializeField] private TextMeshProUGUI answBTMP;
    [SerializeField] private List<string> answersB;
    [Header("Answers C")]
    [SerializeField] private TextMeshProUGUI answCTMP;
    [SerializeField] private List<string> answersC;
    
    private int progressionStep = 0;

    void Start()
    {
        if (questTMP == null) throw new System.Exception("TextMeshProUGUI questTMP null !");
        if (questions.Count == 0) throw new System.Exception("List<string> question is empty !");
        ShowQuestion();
    }

    public void ResetStep()
    {
        progressionStep = 0;
    }

    public void IncrementStep()
    {
        progressionStep++;
        if (progressionStep >= questions.Count) progressionStep = questions.Count;
    }

    public void ShowQuestion()
    {
        if (progressionStep >= questions.Count)
        {
            questTMP.text = "";
            answATMP.text = "";
            answBTMP.text = "";
            answCTMP.text = "";
        }
        else
        {
            questTMP.text = questions[progressionStep];
            answATMP.text = answersA[progressionStep];
            answBTMP.text = answersB[progressionStep];
            answCTMP.text = answersC[progressionStep];
        }
    }

    public void ShowNextQuestion()
    {
        IncrementStep();
        ShowQuestion();
    }
}