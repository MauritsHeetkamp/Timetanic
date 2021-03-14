using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEffect : MonoBehaviour
{
    [SerializeField] UnityEvent onStart;

    // Start is called before the first frame update
    void Start()
    {
        if(onStart != null)
        {
            onStart.Invoke();
        }
    }

}
