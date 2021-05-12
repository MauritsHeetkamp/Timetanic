using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float interactDuration;
    bool canInteract = true;
    [SerializeField] float interactDelay;
    public Player currentInteractingPlayer;

    [SerializeField] string grabParam = "Grab";

    public Popup itemPopup;

    // Checks if this item can be interacted with
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

    // If object can be interacted with
    public virtual void OnResultCanInteract(bool result)
    {

    }

    // Handles interaction
    public virtual void Interact(Player target)
    {
        canInteract = false;
        currentInteractingPlayer = target;
        currentInteractingPlayer.currentUsingInteractable = this;

        if(target.animator != null)
        {
            target.animator.SetTrigger(grabParam);
        }

        CompleteInteract();
    }

     // Starts interaction delay
    public void InteractDelay()
    {
        if(currentInteractingPlayer != null)
        {
            currentInteractingPlayer.FinishedInteract(); // Tells the player its done interacting
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

    // Interaction delay timer
    IEnumerator StartInteractDelay()
    {
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;
    }

    // Cancels interaction
    public virtual void CancelInteract()
    {
        InteractDelay(); // Starts interaction delay
        currentInteractingPlayer = null;
        currentInteractingPlayer.currentUsingInteractable = null;
    }

    // Completes interaction
    public virtual void CompleteInteract()
    {
        InteractDelay(); // Starts interaction delay
        currentInteractingPlayer = null;
        currentInteractingPlayer.currentUsingInteractable = null;
    }

  /*private void FixedUpdate()
    {
        if(itemPopup != null)
        {
            if (canInteract)
            {
                Collider[] players = Physics.OverlapBox(popupZone.position, popupZone.lossyScale / 2, popupZone.rotation, playerLayers, QueryTriggerInteraction.Ignore);

                if (players.Length > 0)
                {
                    itemPopup.SetPopup(true);
                }
                else
                {
                    itemPopup.SetPopup(false);
                }
            }
            else
            {
                itemPopup.SetPopup(false);
            }
        }
    }*/
}
