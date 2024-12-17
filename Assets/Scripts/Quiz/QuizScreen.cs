using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI question;
    [SerializeField] private List<TextMeshProUGUI> answers;

    private string titleText;
    private string questionText;
    private List<string> answersText;

    private void Awake()
    {
        if (answers == null) throw new System.Exception("answers null");
        if (answers.Count != 3) throw new System.Exception("answers.Count != 3");
    }

    private void Start()
    {
        titleText = "";
        questionText = "";
        answersText = new() { "", "", "" };
    }

    public void SetTitle(string title)
    {
        titleText = title;
    }

    public bool SetQuestion(string question, List<string> answers)
    {
        if (answers == null || answers.Count < 3) return false;
        questionText = question;
        for (int i = 0; i < answers.Count; i++)
        {
            answersText[i] = answers[i];
        }
        return true;
    }

    public void UpdateQuestion()
    {
        title.text = titleText;
        question.text = questionText;
        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].text = answersText[i];
        }
    }
}
