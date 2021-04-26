using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MinigameHandler : MonoBehaviour
{
    [SerializeField] Location[] allLocations;
    public List<Minigame> allMinigames;
    public List<Minigame> activeMinigames;

    public int minMinigames, maxMinigames;

    public TaskManager taskHandler;

    public UnityAction onCompleteInit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Initialize()
    {
        foreach(Location location in allLocations)
        {
            foreach(GameObject minigame in location.minigames)
            {
                Minigame thisMinigame = minigame.GetComponent<Minigame>();

                if(thisMinigame != null)
                {
                    allMinigames.Add(thisMinigame);
                }
            }
        }

        StartCoroutine(SpawnMinigames());
    }

    IEnumerator SpawnMinigames()
    {
        yield return null;
        List<Minigame> availableMinigames = new List<Minigame>(allMinigames);
        int minigameAmount = Random.Range(minMinigames, maxMinigames + 1);

        for(int i = 0; i < minigameAmount; i++)
        {
            if(availableMinigames.Count <= 0)
            {
                break;
            }

            int selectedMinigame = Random.Range(0, availableMinigames.Count);
            availableMinigames[selectedMinigame].gameObject.SetActive(true);
            availableMinigames[selectedMinigame].Initialize(this);
            availableMinigames.RemoveAt(selectedMinigame);

            yield return null;
        }

        if (onCompleteInit != null)
        {
            onCompleteInit.Invoke();
        }
    }
}
