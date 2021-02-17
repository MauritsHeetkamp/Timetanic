using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float interactDuration;
    bool canInteract = true;
    [SerializeField] float interactDelay;
    public Player currentInteractingPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool CanInteract(Player askingPlayer)
    {
        if (canInteract && currentInteractingPlayer == null &&  askingPlayer != null && askingPlayer.currentHoldingItem == null)
        {
            OnResultCanInteract(true);
            return true;
        }
        OnResultCanInteract(false);
        return false;
    }

    public virtual void OnResultCanInteract(bool result) //If anything needs to happen based on if the player can interact
    {

    }

    public virtual void Interact(Player target)
    {
        canInteract = false;
        currentInteractingPlayer = target;
        currentInteractingPlayer.currentUsingInteractable = this;
        CompleteInteract();
    }

    public void InteractDelay()
    {
        if(currentInteractingPlayer != null)
        {
            currentInteractingPlayer.FinishedInteract();
        }
        if(interactDelay > 0)
        {
            StartCoroutine(StartInteractDelay());
        }
        else
        {
            canInteract = true;
        }
    }
    IEnumerator StartInteractDelay()
    {
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;
    }

    public virtual void CancelInteract()
    {
        InteractDelay();
        currentInteractingPlayer = null;
        currentInteractingPlayer.currentUsingInteractable = null;
    }

    public virtual void CompleteInteract()
    {
        Debug.Log("INTERACTED");
        InteractDelay();
        currentInteractingPlayer = null;
        currentInteractingPlayer.currentUsingInteractable = null;
    }
}
