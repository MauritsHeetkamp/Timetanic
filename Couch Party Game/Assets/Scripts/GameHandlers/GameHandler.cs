using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Custom.Time;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [Header("CountdownTimer")]
    bool almostFinished; // Is the time almost over?
    [SerializeField] CountdownTimer gameTime; // the timer
    [SerializeField] TimeDuration almostFinishedTime; // Timestamp when the time is almost over
    [SerializeField] Slider timeIndicator;
    Coroutine countdownRoutine; // Countdown coroutine


    [Header("Score")]
    [SerializeField] string scoreName = "Score: ";
    [SerializeField] TextMeshProUGUI scoreText;
    public int score;

    [Header("Spawning")]
    [SerializeField] SpawnManager spawnHandler;
    [SerializeField] MinigameHandler minigameHandler;

    private void Awake()
    {
        if (scoreText != null)
        {
            scoreText.text = scoreName + score.ToString();
        }
        StartGame();
    }

    // Starts the game
    public void StartGame()
    {      
        StartStopCountdown(true, true); // Starts and resets timer
        spawnHandler.GetSpawnData(); // Spawns players
        minigameHandler.Initialize();
    }

    // Starts or stops the countdown
    public void StartStopCountdown(bool start, bool resetOnStart = false)
    {
        if (countdownRoutine != null) // Checks if there is already an countdown running
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        if (start)
        {
            if (resetOnStart)
            {
                gameTime.Reset(); // Resets the timer
            }
            countdownRoutine = StartCoroutine(gameTime.Countdown()); // Starts countdown
        }
    }

    /* Old timer (shows actual time)
    // Updates the timer ui
    public void UpdateTimer()
    {
        string secondsText = gameTime.remainingSeconds < 10 ? "0" + gameTime.remainingSeconds.ToString() : gameTime.remainingSeconds.ToString(); // Gets the seconds string
        gameTimeText.text = gameTime.remainingMinutes + ":" + secondsText; // Gets the minutes string

        if (almostFinished)
        {
            if (gameTime.IsLower(almostFinishedTime)) // Is the almostfinished time lower then the remaining time?
            {
                almostFinished = false;
                if(textAnimator != null && defaultTextAnim != null) // Checks if animations should be stopped
                {
                    textAnimator.Play(defaultTextAnim.name);
                }
            }
        }
        else
        {
            if (gameTime.IsHigher(almostFinishedTime) || gameTime.IsEqual(almostFinishedTime)) // Is the almostfinished time higher or equal to the remaining time?
            {
                almostFinished = true;
                if(textAnimator != null && almostFinishedTextAnim != null) // Checks if animations should be started
                {
                    textAnimator.Play(almostFinishedTextAnim.name);
                }
            }
        }
    }*/

    public void UpdateTimer()
    {
        float remainingSeconds = gameTime.GetRemainingSeconds();
        float totalSeconds = gameTime.duration.GetSeconds();


        if(timeIndicator != null)
        {
            timeIndicator.value = remainingSeconds / totalSeconds;
        }

        if (almostFinished)
        {
            if (gameTime.IsLower(almostFinishedTime)) // Is the almostfinished time lower then the remaining time?
            {
                almostFinished = false;
            }
        }
        else
        {
            if (gameTime.IsHigher(almostFinishedTime) || gameTime.IsEqual(almostFinishedTime)) // Is the almostfinished time higher or equal to the remaining time?
            {
                almostFinished = true;
            }
        }
    }

    // Finishes the game
    public void FinishGame()
    {

    }

    // Modifies the final score
    public void ChangeScore(int value)
    {
        score += value;
        if(scoreText != null)
        {
            scoreText.text = scoreName + score.ToString();
        }
    }
}

namespace Custom.Time
{
    [System.Serializable]
    public class CountdownTimer
    {
        public TimeDuration duration;
        [SerializeField] UnityEvent onCompleteTimer;
        [SerializeField] UnityEvent onTimerChanged;

        public float countdownSpeed = 1; // Countdown speed modifier
        public float remainingSeconds;
        public int remainingMinutes;


        // Resets the timer
        public void Reset()
        {
            remainingSeconds = duration.seconds;
            remainingMinutes = duration.minutes;
            onTimerChanged.Invoke();
        }

        // Countdown routine
        public IEnumerator Countdown()
        {
            while (remainingMinutes != 0 || remainingSeconds != 0) // Is the time over?
            {
                if (remainingSeconds > 0)
                {
                    remainingSeconds -= UnityEngine.Time.deltaTime * countdownSpeed;
                }
                else
                {
                    if (remainingMinutes > 0) // Is there at least one minute left?
                    {
                        remainingMinutes--; // Remove minute
                        remainingSeconds = 59; // Add new seconds
                    }
                    else
                    {
                        remainingSeconds = 0;
                    }
                }
                onTimerChanged.Invoke();
                yield return new WaitForSeconds(UnityEngine.Time.deltaTime);
            }
            onCompleteTimer.Invoke();
        }

        public float GetRemainingSeconds()
        {
            float seconds = 0;

            seconds += remainingMinutes * 60;
            seconds += remainingSeconds;

            return seconds;
        }

        // Adds time to the timer
        public void ChangeTime(TimeDuration time)
        {
            remainingMinutes += time.minutes;
            remainingSeconds += time.seconds;

            if (remainingSeconds >= 60)
            {
                remainingMinutes += Mathf.FloorToInt(remainingSeconds / 60); // How many minutes are in the remaining seconds
                remainingSeconds -= Mathf.FloorToInt(remainingSeconds / 60) * 60; // Extracts the minutes from seconds
                remainingMinutes++;
            }

            onTimerChanged.Invoke();
        }

        // Checks if a time is higher then the current time
        public bool IsHigher(TimeDuration time)
        {
            if (time.minutes > remainingMinutes)
            {
                return true;
            }
            else
            {
                if (time.seconds > remainingSeconds)
                {
                    return true;
                }
            }

            return false;
        }

        // Checks if a time is lower then the current time
        public bool IsLower(TimeDuration time)
        {
            if (time.minutes < remainingMinutes)
            {
                return true;
            }
            else
            {
                if (time.seconds < remainingSeconds)
                {
                    return true;
                }
            }

            return false;
        }

        // Checks if a time is equal to the current time
        public bool IsEqual(TimeDuration time)
        {
            if (time.minutes == remainingMinutes && time.seconds == remainingSeconds)
            {
                return true;
            }

            return false;
        }
    }

    [System.Serializable]
    public struct TimeDuration
    {
        [Range(0, 59)] public float seconds;
        [Range(0, 59)] public int minutes;

        public float GetSeconds()
        {
            float totalSeconds = 0;

            totalSeconds += minutes * 60;
            totalSeconds += seconds;

            return totalSeconds;
        }
    }
}
