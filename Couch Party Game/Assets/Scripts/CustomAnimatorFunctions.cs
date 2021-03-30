using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimatorFunctions : MonoBehaviour
{
    Animator animator;


    public void SetBoolTrue(string boolName)
    {
        Initialize();
        SetBool(boolName, true);
    }

    public void SetBoolFalse(string boolName)
    {
        Initialize();
        SetBool(boolName, false);
    }

    void Initialize()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void SetBool(string boolName, bool value)
    {
        if(animator != null)
        {
            animator.SetBool(boolName, value);
        }
    }
}
