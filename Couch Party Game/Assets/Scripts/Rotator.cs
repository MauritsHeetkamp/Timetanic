using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 rotateAmount;
    [SerializeField] float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateSpeed > 0 && rotateAmount != Vector3.zero)
        {
            transform.Rotate(rotateAmount * Time.deltaTime * rotateSpeed);
        }
    }
}
