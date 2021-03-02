using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
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
}
