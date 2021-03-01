using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSceneInstant(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
