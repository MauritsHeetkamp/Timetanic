using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    [SerializeField] IngameFadeManager fadeManager;
    [SerializeField] List<AsyncScene> loadingScenes = new List<AsyncScene>();

    public void ChangeSceneAsync(string targetScene)
    {
        bool alreadyLoading = false;

        foreach(AsyncScene scene in loadingScenes)
        {
            if(scene.targetScene == targetScene)
            {
                alreadyLoading = true;
            }
        }

        if (!alreadyLoading)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
            loadingScenes.Add(new AsyncScene(targetScene, operation));
        }
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

        public AsyncScene(string _targetScene, AsyncOperation _asyncOperation)
        {
            targetScene = _targetScene;
            asyncOperation = _asyncOperation;
        }
    }
}
