using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using static QuestionSystem.Question;



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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceSuccess;
    [SerializeField] private AudioSource audioSourceFailed;

    private int progressionStep = 0;
    private int correctAnswers = 0;
    private DateTime startTime;
    private DateTime lastTime;

    void OnEnable()
    {
        if (questTMP == null) throw new System.Exception("TextMeshProUGUI questTMP null !");
        if (datas.Count == 0) throw new System.Exception("List<Question> datas is empty !");
        
        buttonA.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.A));
        buttonB.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.B));
        buttonC.onClick.AddListener(() => OnAnswerClicked(Question.GoodAnswer.C));

        winObject.SetActive(false);
        loseObject.SetActive(false);
        titleObject.SetActive(true);

        startTime = DateTime.Now;
        lastTime = DateTime.Now;

        ShowQuestion();
    }

    private void OnDisable()
    {
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        buttonC.onClick.RemoveAllListeners();
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
        lastTime = DateTime.Now;
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
            bool isGoodAnswer = datas[progressionStep].goodAnswer == chosenAnswer;
            SaveQuestion(GetStringAnswer(chosenAnswer), isGoodAnswer);
            if (isGoodAnswer) correctAnswers++;
        }
        ShowNextQuestion();
    }

    private string GetStringAnswer(Question.GoodAnswer chosenAnswer)
    {
        if (chosenAnswer == GoodAnswer.A) return datas[progressionStep].answerA;
        if (chosenAnswer == GoodAnswer.B) return datas[progressionStep].answerB;
        if (chosenAnswer == GoodAnswer.C) return datas[progressionStep].answerC;
        return "NULL";
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

        bool win = correctAnswers >= (datas.Count/2);
        playResultSound(win);
        string resultString = win ? "WIN" : "LOSE";
        SaveFinalResult($"{correctAnswers}/{datas.Count} ({resultString})");
        winObject.SetActive(win);
        loseObject.SetActive(!win);
        GameManager.Instance.datas.SaveData();
    }

    private void playResultSound(bool win)
    {
        if (win)
        {
            audioSourceSuccess.Play();
        }
        else
        {
            audioSourceFailed.Play();
        }
    }

    // Save Datas : "Name;OldState;NewState;TimeStamp;TimeStampSinceStart;Infos"
    public void SaveQuestion(string answer, bool goodAnswer)
    {
        string timeStamp1 = (DateTime.Now/*elapsedTime*/ - lastTime).ToString();
        string timeStamp2 = (DateTime.Now/*elapsedTime*/ - startTime).ToString();
        string goodAnswerString = goodAnswer ? "CORRECT" : "INCORRECT";
        GameManager.Instance.datas.AddData(DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                           "QUESTIONS", $";;{timeStamp1};{timeStamp2};{answer} ({goodAnswerString})");
    }
    public void SaveFinalResult(string result)
    {
        string timeStamp2 = (DateTime.Now/*elapsedTime*/ - startTime).ToString();
        GameManager.Instance.datas.AddData(DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                           "QUESTIONS_RESULT", $";;;{timeStamp2};{result}");
    }
}
