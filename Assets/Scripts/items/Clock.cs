using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NumberRow
{
    public List<GameObject> row = new List<GameObject>(10);
}

public class Clock : MonoBehaviour
{
    [SerializeField] private bool startTimer = false;
    [SerializeField]
    private NumberRow[] numbers = new NumberRow[] {
        new(), new(), new(), new()
    };
    [SerializeField][Range(0, 3600)] private int timerInSeconds = 15;
    [SerializeField] private ClockButton buttonAnimation;

    private float timeRemaining;
    private bool timerRunning = false;

    void Start()
    {
        InitClock();
    }

    private void InitClock()
    {
        timeRemaining = timerInSeconds;
        timerRunning = false;
        startTimer = false;
        UpdateClockDisplay();
    }

    void Update()
    {
        if (startTimer && !timerRunning)
        {
            timerRunning = true;
            startTimer = false;
            buttonAnimation.AnimateClick();
        }

        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateClockDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                timeRemaining = timerInSeconds;
                UpdateClockDisplay();
                // Code à exécuter lorsque le minuteur se termine :
                // NULL
            }
        }
    }

    private void UpdateClockDisplay()
    {
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        int minutes = Mathf.FloorToInt  (timeRemaining / 60);

        int[] digits = new int[4];
        digits[0] = minutes / 10; // Dizaine de minutes
        digits[1] = minutes % 10; // Unité de minutes
        digits[2] = seconds / 10; // Dizaine de secondes
        digits[3] = seconds % 10; // Unité de secondes

        for (int i = 0; i < digits.Length; i++)
        {
            UpdateDigitDisplay(i, digits[i]);
        }
    }

    private void UpdateDigitDisplay(int position, int digit)
    {
        numbers[position].row.ForEach((obj) => obj.SetActive(false));
        if (digit >= 0 && digit < numbers[position].row.Count)
            numbers[position].row[digit].SetActive(true);
    }
}
