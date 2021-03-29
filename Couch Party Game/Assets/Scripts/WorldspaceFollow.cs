using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset, eulers;
    // Start is called before the first frame update
    void Start()
    {
        if (target != null)
        {
            transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            transform.position = target.position + offset;
            transform.eulerAngles = eulers;
        }
    }
}
