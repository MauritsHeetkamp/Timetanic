using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Task : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(TaskData data)
    {
        if(nameText != null)
        {
            nameText.text = data.taskName;
        }
    }

    public void Complete()
    {
        Destroy(gameObject);
    }
}


[System.Serializable]
public struct TaskData
{
    public string taskName;

    public TaskData(string _taskName)
    {
        taskName = _taskName;
    }
}
