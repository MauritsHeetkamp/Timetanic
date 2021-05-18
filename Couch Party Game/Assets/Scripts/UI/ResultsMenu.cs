using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Custom.Time;
using TMPro;

public class ResultsMenu : MonoBehaviour
{
    bool finished;

    [Header("Count Delays")]
    [SerializeField] float delayBeforeStartCount = 3;
    [SerializeField] float maxCountDuration;
    [SerializeField] float maxCountDelay;
    [SerializeField] float delayBetweenCounts;

    [SerializeField] float passedResultDelay;

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

    [SerializeField] ScoreRequirements[] possibleResults;

    [SerializeField] float scorePerPassenger;
    [SerializeField] float remainingTimeMultiplier;


    [Header("UI Elements")]
    [SerializeField] int scoreDecimalAmount;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] TextMeshProUGUI passengersSavedText;
    [SerializeField] TextMeshProUGUI passengersDiedText;
    [SerializeField] TextMeshProUGUI totalScoreText;

    [SerializeField] Image resultGradeImage;
    [SerializeField] Vector2 gradeDefaultScale;
    [SerializeField] GameObject passedObject;
    [SerializeField] Image passedImage;
    [SerializeField] Sprite passedSprite, notPassedSprite;
    [SerializeField] Animator passedAnimator;
    [SerializeField] AnimationClip passedAnim;
    [SerializeField] Sprite defaultGradeSprite;

    [SerializeField] GameObject returnTextObject;

    [Header("Finalize")]
    [SerializeField] UnityEvent onFinishButtonPressed;

    private void Awake()
    {
        gradeDefaultScale = resultGradeImage.rectTransform.sizeDelta;
    }

    private void OnEnable()
    {
        foreach (PlayerData player in PlayerManager.instance.connectedToPCPlayers)
        {
            player.onAnyInputUI += Next;
        }
    }

    private void OnDisable()
    {
        foreach (PlayerData player in PlayerManager.instance.connectedToPCPlayers)
        {
            player.onAnyInputUI -= Next;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Next(InputAction.CallbackContext context, PlayerData player)
    {
        if (context.started && !finished)
        {
            if (currentStepRoutine != null)
            {
                StopCoroutine(currentStepRoutine);
            }

            if (onCompletedStep != null)
            {
                onCompletedStep.Invoke();
            }
        }
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
        resultGradeImage.rectTransform.sizeDelta = gradeDefaultScale;
        resultGradeImage.sprite = defaultGradeSprite;

        passengersSaved = 0;
        passengersDied = 0;
        timer = null;
        totalScore = 0;

        finished = false;

        returnTextObject.SetActive(false);
        passedObject.SetActive(false);

        onCompletedStep = null;
    }

    public void ShowScore(int _passengersSaved, int _passengersDied, CountdownTimer _timer)
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

        if(timer.remainingMinutes > 0 || timer.remainingSeconds > 0)
        {
            if (timer.duration.minutes > 0 || timer.duration.seconds > 0)
            {
                scoreFromTime = totalScore * ((timer.GetRemainingSeconds() / timer.duration.GetSeconds()) * remainingTimeMultiplier);
            }
        }

        totalScore = 0;

    }

    void FinishShowResult()
    {
        finished = true;

        onCompletedStep -= FinishShowResult;

        if(passedAnimator != null)
        {
            passedAnimator.StopPlayback();
        }

        returnTextObject.SetActive(true);

        if (!passedObject.activeSelf)
        {
            passedObject.SetActive(true);
        }


        foreach (PlayerData player in PlayerManager.instance.connectedToPCPlayers)
        {
            player.onAnyInputUI += OnFinishButtonPressed;
        }
    }

    void OnFinishButtonPressed(InputAction.CallbackContext context, PlayerData player)
    {
        if(context.started && finished)
        {
            foreach (PlayerData thisPlayer in PlayerManager.instance.connectedToPCPlayers)
            {
                thisPlayer.onAnyInputUI -= OnFinishButtonPressed;
            }

            if (onFinishButtonPressed != null)
            {
                onFinishButtonPressed.Invoke();
            }
        }
    }

    void ShowResult()
    {
        onCompletedStep -= ShowResult;;

        passengersDiedText.text = passengersDied.ToString();

        totalScore = scoreFromTime + scoreFromPassengers - scoreRemovalFromPassengers;
        totalScoreText.text = totalScore.ToString("F" + scoreDecimalAmount.ToString());

        float savedPercentage = 0;
        int totalPassengers = passengersDied + passengersSaved;

        if(totalPassengers > 0)
        {
            if (passengersSaved > 0)
            {
                savedPercentage = ((float)passengersSaved / (float)totalPassengers) * (float)100;
            }
        }
        else
        {
            savedPercentage = 100;
        }

        Debug.Log(savedPercentage);

        if(possibleResults.Length > 0)
        {
            for (int i = 0; i < possibleResults.Length; i++)
            {
                ScoreRequirements thisResult = possibleResults[i];
                if(savedPercentage >= thisResult.savedPassengerPercentage)
                {
                    resultGradeImage.rectTransform.sizeDelta = thisResult.spriteSize;
                    resultGradeImage.sprite = thisResult.resultGrade;
                    passedImage.sprite = thisResult.passed ? passedSprite : notPassedSprite;
                    if(passedAnimator != null)
                    {
                        onCompletedStep += FinishShowResult;
                        currentStepRoutine = StartCoroutine(ShowPassedRoutine(thisResult));
                    }
                    else
                    {
                        FinishShowResult();
                    }
                    break;
                }
            }
        }


        Debug.Log("FINISHED");
    }

    IEnumerator ShowPassedRoutine(ScoreRequirements result)
    {
        yield return new WaitForSeconds(passedResultDelay);
        passedObject.SetActive(true);
        passedAnimator.Play(passedAnim.name);
        yield return new WaitForSeconds(passedAnim.length);

        if(onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    IEnumerator CalculateRemainingTimeRoutine()
    {
        yield return new WaitForSeconds(delayBeforeStartCount);

        if(timer.remainingMinutes > 0 || timer.remainingSeconds > 0)
        {
            int minutes = 0;
            float seconds = 0;

            int multiplier = 1;
            float remainingSeconds = timer.GetRemainingSeconds();

            float countDelay = maxCountDuration / remainingSeconds;

            if (countDelay > maxCountDelay)
            {
                countDelay = maxCountDelay;
            }

            if (countDelay < Time.deltaTime)
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

                if (seconds >= 59)
                {
                    minutes++;
                    seconds -= 59;
                }

                if (minutes > timer.remainingMinutes)
                {
                    minutes = timer.remainingMinutes;
                }

                if (minutes == timer.remainingMinutes && seconds > timer.remainingSeconds)
                {
                    seconds = timer.remainingSeconds;
                }

                string secondsText = seconds < 10 ? "0" + seconds.ToString("F0") : seconds.ToString("F0"); // Gets the seconds string

                timeLeftText.text = minutes + ":" + secondsText;


                if (scoreFromTime != 0)
                {
                    totalScore += scoreFromTime / remainingSeconds; ;
                    totalScoreText.text = totalScore.ToString("F" + scoreDecimalAmount.ToString());
                }
            }

            yield return new WaitForSeconds(delayBetweenCounts);
        }

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    void CalculateAlivePassengers()
    {
        string secondsText = timer.remainingSeconds < 10 ? "0" + timer.remainingSeconds.ToString("F0") : timer.remainingSeconds.ToString("F0"); // Gets the seconds string

        timeLeftText.text = timer.remainingMinutes + ":" + secondsText;

        totalScore = scoreFromTime;
        totalScoreText.text = scoreFromTime.ToString("F" + scoreDecimalAmount.ToString());

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
            totalScoreText.text = totalScore.ToString("F" + scoreDecimalAmount.ToString());
        }

        yield return new WaitForSeconds(delayBetweenCounts);

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    void CalculateDeadPassengers()
    {
        passengersSavedText.text = passengersSaved.ToString();
        totalScore = scoreFromTime + scoreFromPassengers;
        totalScoreText.text = totalScore.ToString("F" + scoreDecimalAmount.ToString());

        onCompletedStep -= CalculateDeadPassengers;
        onCompletedStep += ShowResult;
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
            totalScoreText.text = totalScore.ToString("F" + scoreDecimalAmount.ToString());
        }

        yield return new WaitForSeconds(delayBetweenCounts);

        if (onCompletedStep != null)
        {
            onCompletedStep.Invoke();
        }
    }

    [System.Serializable]
    struct ScoreRequirements
    {
        public float savedPassengerPercentage;
        public Sprite resultGrade;
        public Vector2 spriteSize;
        public bool passed;
    }
}
