using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtuingishMinigame : Minigame
{
    List<Extuingishable> extuingishableFires;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.onExtuingished += RemoveExtuingishable;
        }
    }

    void RemoveExtuingishable(Extuingishable target)
    {
        extuingishableFires.Remove(target);
        if(extuingishableFires.Count == 0)
        {
            FinishMinigame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
