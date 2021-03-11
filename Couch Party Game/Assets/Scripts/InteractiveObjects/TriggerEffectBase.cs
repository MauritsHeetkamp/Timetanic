using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEffectBase : MonoBehaviour
{
    [SerializeField] bool allowPlayers = true; // Should only players be able to trigger this effect
    [SerializeField] string[] allowedTags;

    public bool ValidTarget(Collider target)
    {
        if (!target.isTrigger)
        {
            foreach (string tag in allowedTags)
            {
                if (tag == target.tag)
                {
                    return true;
                }
            }

            if (allowPlayers && target.GetComponent<Player>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
