using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Custom.Time;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Custom.Audio;
using UnityEngine.Video;

public class GameHandler : MonoBehaviour
{
    [SerializeField] IngameFadeManager fadeManager;
    FadePanel currentFadePanel;

    [Header("CountdownTimer")]
    [SerializeField] bool timeBasedOnPlayerAmount = true;
    bool almostFinished; // Is the time almost over?
    [SerializeField] CountdownTimer gameTime; // the timer
    [SerializeField] TimeDuration almostFinishedTime; // Timestamp when the time is almost over

    [SerializeField] UnityEvent onAlmostFinished, onNotAlmostFinished;

    [SerializeField] Slider timeIndicator;
    [SerializeField] TextMeshProUGUI gameTimeText;
    Coroutine countdownRoutine; // Countdown coroutine

    [SerializeField] Animator shipAnimator;
    [SerializeField] AnimationClip sinkAnim;
    //float playbackSpeed = 1;

    [Header("Score")]
    [SerializeField] GameObject resultsMenu;
    [SerializeField] ResultsMenu resultsMenuScript;
    public Score savedPassengers, deadPassengers;

    [Header("Spawning")]
    [SerializeField] UIManager uiManager;
    [SerializeField] SceneSwapManager sceneSwapper;
    public NPCSpawner npcSpawner;
    [SerializeField] PlayerSpawner playerSpawner;
    [SerializeField] MinigameHandler minigameSpawner;
    [SerializeField] VoicelineHandler voicelineHandler;

    [Header("Cutscenes")]
    [SerializeField] CutsceneHandler cutsceneHandler;
    [SerializeField] Cutscene.CutsceneData cutsceneData;

    [Header("StartOfGame")]
    public NotificationHandler notificationHandler;
    [SerializeField] NotificationHandler.NotificationData startOfGameNotification;
    [SerializeField] string characterControlScheme = "Player";
    [SerializeField] AudioPrefab gameMusic;

    private void Start()
    {
        foreach(PlayerData owner in PlayerManager.instance.connectedToLobbyPlayers)
        {
            owner.SwapInputScheme(characterControlScheme); // Sets the control scheme to player controls
        }

        float timerSeconds = gameTime.duration.GetSeconds();

        if (timeBasedOnPlayerAmount)
        {
            gameTime.duration.SetSeconds(timerSeconds / PlayerManager.instance.connectedToLobbyPlayers.Count);
        }

        if(shipAnimator != null && timerSeconds > 0)
        {
            float playbackSpeed = sinkAnim.length / 600;//timerSeconds;
            if(PlayerManager.instance.connectedToLobbyPlayers.Count > 1)
            {
                playbackSpeed *= PlayerManager.instance.connectedToLobbyPlayers.Count * 0.6f;
            }

            shipAnimator.speed = shipAnimator.speed * playbackSpeed;
        }

        StartCutscene();
    }

    public void StartCutscene()
    {
        if(cutsceneHandler != null && cutsceneData.video != null)
        {
            if(SoundManager.instance != null && cutsceneData.audio.clip != null)
            {
                SoundManager.instance.EndBackgroundMusic();
                //SoundManager.instance.SetBackgroundMusic(cutsceneData.audio, false);
            }

            Cutscene newCutscene = cutsceneHandler.StartNewCutscene(cutsceneData);

            if (fadeManager == null)
            {
                IngameFadeManager fadeManager = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<IngameFadeManager>(); // Finds fade handler
            }

            if (sceneSwapper.fader != null)
            {
            sceneSwapper.fadeManager.FadeOut(sceneSwapper.fader);
            }
    
            newCutscene.onCompletedCutscene += PrepareStartGame;
            newCutscene.onCompletedCutscene += () => Destroy(newCutscene.gameObject);
        }
        else
        {
            StartGame();
        }
    }

    public void PrepareStartGame()
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.SetBackgroundMusic(gameMusic, true);
            //SoundManager.instance.ShuffleBackgroundAudio(false);
        }
        Debug.Log("PREP1");
        if(fadeManager != null)
        {
            Debug.Log("PREP2");
            currentFadePanel = fadeManager.FadeIn(0, false);
            currentFadePanel.onFadedIn += StartGame;
        }
    }

    // Starts the game
    public void StartGame()
    {
        Debug.Log("STARTED");

        if(currentFadePanel != null)
        {
            currentFadePanel.onFadedIn -= StartGame;
        }
        StartStopCountdown(true, true); // Starts and resets timer

        npcSpawner.onCompletedSpawn += playerSpawner.GetSpawnData;
        playerSpawner.onCompletedSpawn += minigameSpawner.Initialize;
        minigameSpawner.onCompleteInit += OnInitFinished;
        npcSpawner.StartSpawnNPC();
    }

    public void OnInitFinished()
    {
        npcSpawner.onCompletedSpawn -= playerSpawner.GetSpawnData;
        playerSpawner.onCompletedSpawn -= minigameSpawner.Initialize;
        minigameSpawner.onCompleteInit -= OnInitFinished;
        uiManager.canvasGroup.alpha = 1;

        if(voicelineHandler != null)
        {
            voicelineHandler.InitVoicelineHandler();
            voicelineHandler.StartStopAnnouncer(true);
        }

        if(fadeManager != null && currentFadePanel != null)
        {
            fadeManager.FadeOut(currentFadePanel);
        }
        else
        {
            sceneSwapper.fadeManager.FadeOut(sceneSwapper.fader);
        }

        if(notificationHandler != null)
        {
            notificationHandler.ActivateNotification(startOfGameNotification);
        }

        if(shipAnimator != null && sinkAnim != null)
        {
            shipAnimator.SetBool("Sink", true);
        }
    }

    // Starts or stops the countdown
    public void StartStopCountdown(bool start, bool resetOnStart = false)
    {
        if (countdownRoutine != null) // Checks if there is already an countdown running
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
            if (shipAnimator != null)
            {
                //shipAnimator.speed = 0;
            }
        }

        if (start)
        {
            if (resetOnStart)
            {
                gameTime.Reset(); // Resets the timer
            }
            countdownRoutine = StartCoroutine(gameTime.Countdown()); // Starts countdown
            if (shipAnimator != null)
            {
                //shipAnimator.Play(sinkAnim.name);
            }
        }
    }

    public void UpdateTimer()
    {
        float remainingSeconds = gameTime.GetRemainingSeconds();
        float totalSeconds = gameTime.duration.GetSeconds();

        if(gameTimeText != null)
        {
            string secondsText = gameTime.remainingSeconds < 9.5f ? "0" + gameTime.remainingSeconds.ToString("F0") : gameTime.remainingSeconds.ToString("F0"); // Gets the seconds string
            gameTimeText.text = gameTime.remainingMinutes + ":" + secondsText; // Gets the minutes string
        }

        if (timeIndicator != null)
        {
            timeIndicator.value =  1 - (remainingSeconds / totalSeconds);
        }

        if (almostFinished)
        {
            if (!gameTime.IsEqual(almostFinishedTime) && gameTime.IsLower(almostFinishedTime)) // Is the almostfinished time lower then the remaining time?
            {
                Debug.Log("WAS LOWER M9");
                almostFinished = false;

                if (onNotAlmostFinished != null)
                {
                    onNotAlmostFinished.Invoke();
                }
            }
        }
        else
        {
            if (gameTime.IsHigher(almostFinishedTime) || gameTime.IsEqual(almostFinishedTime)) // Is the almostfinished time higher or equal to the remaining time?
            {
                Debug.Log("FINISH IT");
                almostFinished = true;

                if (onAlmostFinished != null)
                {
                    onAlmostFinished.Invoke();
                }
            }
        }
    }

    // Finishes the game
    public void FinishGame()
    {
        StartStopCountdown(false);

        foreach(Player player in playerSpawner.globalPlayers)
        {
            player.Disable(true);
        }

        if(voicelineHandler != null)
        {
            voicelineHandler.StartStopAnnouncer(false);
        }

        npcSpawner.onKilledAllNpcs += ShowResults;
        npcSpawner.KillAliveNPCS();
    }

    void ShowResults()
    {
        npcSpawner.onKilledAllNpcs -= ShowResults;
        resultsMenu.SetActive(true);
        resultsMenuScript.ShowScore(savedPassengers.currentScore, deadPassengers.currentScore, gameTime);
    }


    // Modifies the final score
    public void PassengerSaved()
    {
        savedPassengers.ChangeScore(1);
        npcSpawner.aliveNPCCounter.ChangeScore(-1);
    }

    public void PassengerDied()
    {
        deadPassengers.ChangeScore(1);
        npcSpawner.aliveNPCCounter.ChangeScore(-1);
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
            while (remainingMinutes > 0 || remainingSeconds > 0) // Is the time over?
            {
                yield return new WaitForSeconds(UnityEngine.Time.deltaTime);
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
                if (time.minutes == remainingMinutes && time.seconds > remainingSeconds)
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
                if (time.minutes == remainingMinutes && time.seconds < remainingSeconds)
                {
                    return true;
                }
            }

            return false;
        }

        // Checks if a time is equal to the current time
        public bool IsEqual(TimeDuration time)
        {
            if (time.minutes == remainingMinutes && Mathf.RoundToInt(time.seconds) == Mathf.RoundToInt(remainingSeconds))
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

        public void SetSeconds(float _seconds)
        {
            minutes = Mathf.FloorToInt(_seconds / 60);
            seconds = _seconds % 60;
        }
    }
}
