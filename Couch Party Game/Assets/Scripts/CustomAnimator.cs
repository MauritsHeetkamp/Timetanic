using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    Animator animator;
    [SerializeField] bool saveState;


    private void OnEnable()
    {
        Initialize();

        if (saveState)
        {
            if(animator != null)
            {
                animator.keepAnimatorControllerStateOnDisable = true;
            }
        }
    }

    private void OnDisable()
    {
        
    }

    public void SetBoolTrue(string boolName)
    {
        SetBool(boolName, true);
    }

    public void SetBoolFalse(string boolName)
    {
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
