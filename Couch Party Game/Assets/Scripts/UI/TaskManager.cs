using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] PlayerSpawner spawnHandler;
    [SerializeField] CameraHandler cameraHandler;
    [SerializeField] GameObject taskPrefab;
    [SerializeField] Transform globalTaskHolder;
    [SerializeField] GameObject globalTaskMasterObject;
    [SerializeField] MinigameHandler minigames;

    [SerializeField] TaskData[] data;
    // Start is called before the first frame update
    void Awake()
    {
        if(cameraHandler != null)
        {
            cameraHandler.onSplitStateChanged += OnScreenStateChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnScreenStateChanged(bool split)
    {
        globalTaskMasterObject.SetActive(!split);
    }

    public Task[] AddTask(TaskData data)
    {
        List<Task> allNewTasks = new List<Task>();

        GameObject newTask = Instantiate(taskPrefab, globalTaskHolder);
        Task taskComponent = newTask.GetComponent<Task>();
        taskComponent.Initialize(data);
        allNewTasks.Add(taskComponent);

        foreach(Player player in spawnHandler.localPlayers)
        {
            if(player.attachedSplitscreen != null)
            {
                GameObject newSplitscreenTask = Instantiate(taskPrefab, player.attachedSplitscreen.taskHolder);
                Task newSplitscreenTaskComponent = newSplitscreenTask.GetComponent<Task>();
                newSplitscreenTaskComponent.Initialize(data);
                allNewTasks.Add(newSplitscreenTaskComponent);
            }
        }

        return allNewTasks.ToArray();
    }
}
