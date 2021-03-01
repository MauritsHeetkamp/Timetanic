using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtuingishMinigame : Minigame
{
    [SerializeField] List<Extuingishable> extuingishableFires;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.onExtuingished += CheckExtuingishables;
        }
    }

    void CheckExtuingishables()
    {
        Debug.Log("CHECKING");
        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            if(extuingishable != null && extuingishable.health > 0 || extuingishable == null)
            {
                return;
            }
        }

        Debug.Log("PASSED");
        FinishMinigame();
    }

    public override void FinishMinigame()
    {
        base.FinishMinigame();

        foreach(Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Disable();
        }

        Debug.Log("COMPLETE");
    }
    public override void StartMinigame()
    {
        base.StartMinigame();
        foreach (Extuingishable extuingishable in extuingishableFires)
        {
            extuingishable.Reset();
        }
    }

    public override void Reset()
    {
        base.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
