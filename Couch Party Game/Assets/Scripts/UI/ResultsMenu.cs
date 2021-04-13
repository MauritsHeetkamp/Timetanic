using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Custom.Time;
using TMPro;

public class ResultsMenu : MonoBehaviour
{
    [Header("Count Delays")]
    [SerializeField] float maxCountDuration;
    [SerializeField] float maxCountDelay;
    [SerializeField] float delayBetweenCounts;

    UnityAction onCompletedStep;
    Coroutine currentStepRoutine;

    int passengersSaved;
    int passengersDied;
    CountdownTimer timer;

    [Header("Score Calculation")]
    float totalScore;
    float scoreFromPassengers;
    float scoreRemovalFromPassengers;
    float scoreFromTime;

    [SerializeField] float scorePerPassenger;
    [SerializeField] float remainingTimeMultiplier;


    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] TextMeshProUGUI passengersSavedText;
    [SerializeField] TextMeshProUGUI passengersDiedText;
    [SerializeField] TextMeshProUGUI totalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        CountdownTimer timer = new CountdownTimer();

        timer.duration.minutes = 5;
        timer.duration.seconds = 30;
        timer.remainingMinutes = 4;
        timer.remainingSeconds = 28;

        ShowScore(200, 300, timer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Reset()
    {
        if(currentStepRoutine != null)
        {
            StopCoroutine(currentStepRoutine);
        }

        timeLeftText.text = "0:00"; ;
        passengersSavedText.text = "0";
        passengersDiedText.text = "0";
        totalScoreText.text = "0";

        passengersSaved = 0;
        passengersDied = 0;
        timer = null;
        totalScore = 0;

        onCompletedStep = null;
    }

    void ShowScore(int _passengersSaved, int _passengersDied, CountdownTimer _timer)
    {
        Reset();

        passengersSaved = _passengersSaved;
        passengersDied = _passengersDied;
        timer = _timer;

        CalculateScore();

        onCompletedStep += CalculateAlivePassengers;
        currentStepRoutine = StartCoroutine(CalculateRemainingTimeRoutine());
    }

    void CalculateScore()
    {
        int totalPassengers = passengersSaved + passengersDied;

        scoreFromPassengers = passengersSaved * scorePerPassenger;

        if(passengersDied > 0 && totalPassengers > 0)
        {
            float percentageDied = (float)passengersDied / (float)totalPassengers;
            scoreRemovalFromPassengers = scoreFromPassengers * percentageDied;
        }

        totalScore = scoreFromPassengers - scoreRemovalFromPassengers;

        Debug.Log(timer.GetRemainingSeconds());
        Debug.Log(timer.duration.GetSeconds());

        if(timer.duration.minutes > 0 || timer.duration.seconds > 0)
        {
            scoreFromTime = totalScore + ((timer.GetRemainingSeconds() / timer.duration.GetSeconds()) * remainingTimeMultiplier);
        }

        totalScore = 0;

    }

    void FinishResult()
    {
        onCompletedStep -= FinishResult;
        Debug.Log("FINISHED");
    }

    IEnumerator CalculateRemainingTimeRoutine()
    {
        int minutes = 0;
        float seconds = 0;

        int multiplier = 1;
        float remainingSeconds = timer.GetRemainingSeconds();

        float countDelay = maxCountDuration / remainingSeconds;

        if(countDelay > maxCountDelay)
        {
            countDelay = maxCountDelay;
        }

        if(countDelay < Time.deltaTime)
        {
            multiplier = Mathf.CeilToInt(Time.deltaTime / countDelay);
        }

        remainingSeconds = Mathf.CeilToInt(remainingSeconds / multiplier);

        for (int i = 0; i < remainingSeconds; i++)
        {
            yield return new WaitForSeconds(countDelay);
            /*if(timer.remainingSeconds > 0)
            {
                timer.remainingSeconds--;
            }
            else
            {
                if(timer.remainingMinutes > 0)
                {
                    timer.remainingMinutes--;
                    timer.remainingSeconds = 59;
                }
            }*/

            minutes += Mathf.FloorToInt(multiplier / 60);
            seconds += multiplier % 60;

            if(seconds >= 59)
            {
                minutes++;
                seconds -= 59;
            }

            if(minutes > timer.remainingMinutes)
            {
                minutes = timer.remainingMinutes;
            }

            if(minutes == timer.remainingMinutes && seconds > timer.remainingSeconds)
            {
                seconds = timer.remainingSeconds;
            }

            string secondsText = seconds < 10 ? "0" + seconds.ToString("F0") : seconds.ToString("F0"); // Gets the seconds string

            timeLeftText.text = minutes + ":" + secondsText;


            if(scoreFromTime != 0)
            {
                totalScore += scoreFromTime / remainingSeconds; ;
                totalScoreText.text = totalScore.ToString("F0");
            }
        }

        yield return new WaitForSeconds(delayBetweenCounts);

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    void CalculateAlivePassengers()
    {
        onCompletedStep -= CalculateAlivePassengers;
        onCompletedStep += CalculateDeadPassengers;
        currentStepRoutine = StartCoroutine(CalculateAlivePassengersRoutine());
    }

    IEnumerator CalculateAlivePassengersRoutine()
    {
        int multiplier = 1;
        float countDelay = maxCountDuration / passengersSaved;

        if (countDelay > maxCountDelay)
        {
            countDelay = maxCountDelay;
        }

        if (countDelay < Time.deltaTime)
        {
            multiplier = Mathf.RoundToInt(Time.deltaTime / countDelay);
        }

        int countAmount = Mathf.CeilToInt(passengersSaved / multiplier);

        for (int i = 1; i <= countAmount; i++)
        {
            yield return new WaitForSeconds(countDelay);

            int passengersSavedAmount = i * multiplier;

            if (passengersSavedAmount > passengersSaved)
            {
                passengersSavedAmount = passengersSaved;
            }

            passengersSavedText.text = passengersSavedAmount.ToString();

            totalScore += scoreFromPassengers / countAmount;
            totalScoreText.text = totalScore.ToString("F0");
        }

        yield return new WaitForSeconds(delayBetweenCounts);

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    void CalculateDeadPassengers()
    {
        onCompletedStep -= CalculateDeadPassengers;
        onCompletedStep += FinishResult;
        currentStepRoutine = StartCoroutine(CalculateDeadPassengersRoutine());
    }

    IEnumerator CalculateDeadPassengersRoutine()
    {
        int multiplier = 1;
        float countDelay = maxCountDuration / passengersDied;

        if (countDelay > maxCountDelay)
        {
            countDelay = maxCountDelay;
        }

        if (countDelay < Time.deltaTime)
        {
            multiplier = Mathf.RoundToInt(Time.deltaTime / countDelay);
        }

        int countAmount = Mathf.CeilToInt(passengersDied / multiplier);

        for (int i = 1; i <= countAmount; i++)
        {
            int passengersDiedAmount = i * multiplier;

            yield return new WaitForSeconds(countDelay);

            if(passengersDiedAmount > passengersDied)
            {
                passengersDiedAmount = passengersDied;
            }

            passengersDiedText.text = passengersDiedAmount.ToString();

            totalScore -= scoreRemovalFromPassengers / countAmount;
            totalScoreText.text = totalScore.ToString("F0");
        }

        yield return new WaitForSeconds(delayBetweenCounts);

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }
}
