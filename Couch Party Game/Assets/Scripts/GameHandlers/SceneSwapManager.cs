using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    [SerializeField] bool unfadeOnStart;

    public static SceneSwapManager instance;

    [SerializeField] FadeManager fadeManager;
    [SerializeField] List<AsyncScene> loadingScenes = new List<AsyncScene>();


    bool canBeUsed = true;
    FadePanel fader;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null)
        {
            GameObject previousInstance = instance.gameObject;

            if(instance.fader != null)
            {
                fader = instance.fader;
                fader.transform.parent = fadeManager.targetParent;
            }

            instance = this;

            if (unfadeOnStart)
            {
                if(instance.fader != null)
                {
                    fadeManager.FadeOut(fader);
                }
                else
                {
                    fadeManager.FadeOut();
                }
            }

            Destroy(previousInstance);
        }
        else
        {
            instance = this;
        }
    }


#if UNITY_STANDALONE && !UNITY_EDITOR
    public void PreloadScene(string targetScene)
    {
        foreach (AsyncScene scene in loadingScenes)
        {
            if (scene.targetScene == targetScene)
            {
                return;
            }
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;
        loadingScenes.Add(new AsyncScene(targetScene, operation));
    }
#endif

    public void ChangeSceneAsync(string targetScene)
    {
        if (!canBeUsed)
        {
            return;
        }

        AsyncScene targetOperation = null;

        foreach(AsyncScene scene in loadingScenes)
        {
            if(scene.targetScene == targetScene)
            {
                targetOperation = scene;
            }
        }

        if (targetOperation == null)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
            operation.allowSceneActivation = false;
            targetOperation = new AsyncScene(targetScene, operation);
            loadingScenes.Add(targetOperation);
        }

        if(targetOperation != null)
        {
            fader = fadeManager.FadeIn(0, false);

            fader.onFadedIn += () => EnableAsyncSceneswap(targetOperation);
        }
    }

    public void EnableAsyncSceneswap(AsyncScene operation)
    {
        operation.asyncOperation.allowSceneActivation = true;
    }

    // Changes scene instantly
    public void ChangeSceneInstant(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    // Closes the game
    public void CloseGame()
    {
        Application.Quit();
    }

    [System.Serializable]
    public class AsyncScene
    {
        public string targetScene;
        public AsyncOperation asyncOperation;
        public UnityAction onLoaded;

        public AsyncScene(string _targetScene, AsyncOperation _asyncOperation)
        {
            targetScene = _targetScene;
            asyncOperation = _asyncOperation;
        }
    }
}
