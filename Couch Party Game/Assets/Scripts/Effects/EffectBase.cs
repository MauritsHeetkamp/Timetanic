using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    public virtual void ApplyEffect()
    {

    }

    public virtual void RemoveEffect()
    {

    }

    private void OnDestroy()
    {
        RemoveEffect();
    }
}
