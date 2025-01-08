using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private Button buttonA;

    [Header("Answer B")]
    [SerializeField] private TextMeshProUGUI answBTMP;
    [SerializeField] private Button buttonB;

    [Header("Answer C")]
    [SerializeField] private TextMeshProUGUI answCTMP;
    [SerializeField] private Button buttonC;

    [Header("Datas")]
    [SerializeField] private List<Question> datas;

    [Header("Result Objects")]
    [SerializeField] private GameObject winObject;
    [SerializeField] private GameObject loseObject;
    [SerializeField] private GameObject titleObject;

    private int progressionStep = 0;
    private int correctAnswers = 0;

    void Start()
    {
        if (questTMP == null) throw new System.Exception("TextMeshProUGUI questTMP null !");
        if (datas.Count == 0) throw new System.Exception("List<Question> datas is empty !");
        
        buttonA.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.A));
        buttonB.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.B));
        buttonC.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.C));

        winObject.SetActive(false);
        loseObject.SetActive(false);
        titleObject.SetActive(true);
        ShowQuestion();
    }

    public void ResetStep()
    {
        progressionStep = 0;
        correctAnswers = 0;
        winObject.SetActive(false);
        loseObject.SetActive(false);
        titleObject.SetActive(true);
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
            CheckResult();
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

    private void OnAnswerClicked(Question.GoodAnswer chosenAnswer)
    {
        if (progressionStep < datas.Count)
        {
            if (datas[progressionStep].goodAnswer == chosenAnswer)
            {
                Debug.Log("Bonne réponse !");
                correctAnswers++;
            }
            else
            {
                Debug.Log("Mauvaise réponse.");
            }
        }
        ShowNextQuestion();
    }

    private void CheckResult()
    {
        questTMP.text = "";
        answATMP.text = "";
        answBTMP.text = "";
        answCTMP.text = "";
        

        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
        buttonC.gameObject.SetActive(false);

        titleObject.SetActive(false);
        
        if (correctAnswers >= 4)
        {
            winObject.SetActive(true);
            Debug.Log("Vous avez gagné !");
        }
        else
        {
            loseObject.SetActive(true);
            Debug.Log("Vous avez perdu.");
        }
    }
}
