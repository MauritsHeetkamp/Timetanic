using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Custom.Time;
using UnityEngine;
using TMPro;

public class GameHandler : MonoBehaviour
{
    [Header("CountdownTimer")]
    bool almostFinished;
    [SerializeField] CountdownTimer gameTime;
    [SerializeField] TimeDuration almostFinishedTime;
    [SerializeField] TextMeshProUGUI gameTimeText;
    [SerializeField] Animator textAnimator;
    [SerializeField] AnimationClip defaultTextAnim, almostFinishedTextAnim;
    Coroutine countdownRoutine;


    [Header("Score")]
    int score;

    [Header("Spawning")]
    [SerializeField] SpawnManager spawnHandler;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
    }

    public void StartGame()
    {      
        StartStopCountdown(true, true);
        spawnHandler.GetSpawnData();
    }

    public void StartStopCountdown(bool start, bool resetOnStart = false)
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        if (start)
        {
            if (resetOnStart)
            {
                gameTime.Reset();
            }
            countdownRoutine = StartCoroutine(gameTime.Countdown());
        }
    }

    public void UpdateTimer()
    {
        string secondsText = gameTime.remainingSeconds < 10 ? "0" + gameTime.remainingSeconds.ToString() : gameTime.remainingSeconds.ToString();
        gameTimeText.text = gameTime.remainingMinutes + ":" + secondsText;

        if (almostFinished)
        {
            if (gameTime.IsLower(almostFinishedTime))
            {
                almostFinished = false;
                if(textAnimator != null && defaultTextAnim != null)
                {
                    textAnimator.Play(defaultTextAnim.name);
                }
            }
        }
        else
        {
            if (gameTime.IsHigher(almostFinishedTime) || gameTime.IsEqual(almostFinishedTime))
            {
                almostFinished = true;
                if(textAnimator != null && almostFinishedTextAnim != null)
                {
                    textAnimator.Play(almostFinishedTextAnim.name);
                }
            }
        }
    }

    public void FinishGame()
    {

    }

    public void ChangeScore(int value)
    {
        score += value;
    }
}

namespace Custom.Time
{
    [System.Serializable]
    public class CountdownTimer
    {
        [SerializeField] TimeDuration duration;
        [SerializeField] UnityEvent onCompleteTimer;
        [SerializeField] UnityEvent onTimerChanged;

        public float countdownSpeed = 1;
        public int remainingSeconds, remainingMinutes;

        public void Reset()
        {
            remainingSeconds = duration.seconds;
            remainingMinutes = duration.minutes;
            onTimerChanged.Invoke();
        }

        public IEnumerator Countdown()
        {
            while (remainingMinutes != 0 || remainingSeconds != 0)
            {
                if (remainingSeconds > 0)
                {
                    remainingSeconds--;
                }
                else
                {
                    if (remainingMinutes > 0)
                    {
                        remainingMinutes--;
                        remainingSeconds = 59;
                    }
                }
                onTimerChanged.Invoke();
                yield return new WaitForSeconds(1 / countdownSpeed);
            }
            onCompleteTimer.Invoke();
        }

        public void ChangeTime(TimeDuration time)
        {
            remainingMinutes += time.minutes;
            remainingSeconds += time.seconds;

            if (remainingSeconds >= 60)
            {
                remainingMinutes += Mathf.FloorToInt(remainingSeconds / 60);
                remainingSeconds -= Mathf.FloorToInt(remainingSeconds / 60) * 60;
                remainingMinutes++;
            }

            onTimerChanged.Invoke();
        }

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
        [Range(0, 59)] public int seconds;
        [Range(0, 59)] public int minutes;
    }
}
