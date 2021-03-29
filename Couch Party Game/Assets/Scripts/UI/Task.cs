using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image locationIcon;


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
            locationIcon.sprite = data.taskLocationIcon;
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
    public Sprite taskLocationIcon;
    public TaskData(string _taskName, Sprite _taskLocationIcon)
    {
        taskName = _taskName;
        taskLocationIcon = _taskLocationIcon;
    }
}
