using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Audio;


public class Grabbable : Interactable
{
    public Rigidbody rigid;
    public string itemName;
    [SerializeField] Vector3 itemLocalPosition, itemLocalEulers; // Position and Rotation data
    [SerializeField] Transform leftHandle, rightHandle; // Handles for Inverse Kinematics (IK) rigs
    public string holdingParam;

    [SerializeField] Vector3 raycastOffset = new Vector3(0, 0.1f, 0);
    [SerializeField] ThreeDAudioPrefab dropSFX;
    [SerializeField] float dropCheckRange = 1;
    [SerializeField] LayerMask hittableLayers;

    [SerializeField] float sfxDropDelay = 0.25f;
    bool canSFX = true;

    // Override on completed interaction
    public override void CompleteInteract()
    {
        if(itemPopup != null)
        {
            itemPopup.SetPopup(false);
        }
        currentInteractingPlayer.currentUsingInteractable = null;
        Attach();
        currentInteractingPlayer.FinishedInteract();
    }

    // Attaches grabbable to player
    public virtual void Attach()
    {
        rigid.isKinematic = true; // Disables physics
        GetComponent<Collider>().enabled = false;
        currentInteractingPlayer.currentHoldingItem = this; // Lets player know its holding an item
        transform.parent = currentInteractingPlayer.transform;
        transform.localPosition = itemLocalPosition;
        transform.localEulerAngles = itemLocalEulers;

        if (currentInteractingPlayer.animator != null && !string.IsNullOrEmpty(holdingParam))
        {
            currentInteractingPlayer.animator.SetBool(holdingParam, true);
        }
    }

    // Disattached grabbable from player
    public virtual void Disattach()
    {
        if (currentInteractingPlayer.animator != null && !string.IsNullOrEmpty(holdingParam))
        {
            currentInteractingPlayer.animator.SetBool(holdingParam, false);
        }
        transform.parent = null;
        rigid.isKinematic = false; // Enables physics
        currentInteractingPlayer.currentHoldingItem = null; // Lets player know its not holding an item
        GetComponent<Collider>().enabled = true;
        currentInteractingPlayer.FinishedInteract(); // Lets player know it finished interacting
        InteractDelay(); // Starts interaction delay
        currentInteractingPlayer = null;
    }

    // Destroys item
    public virtual void Break()
    {
        currentInteractingPlayer.currentHoldingItem = null;
        currentInteractingPlayer.FinishedInteract(); // Lets player know it finished interacting
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(canSFX && Physics.Raycast(transform.position + raycastOffset, Vector3.down, dropCheckRange, hittableLayers))
        {
            if(SoundManager.instance != null && dropSFX.clip != null)
            {
                canSFX = false;
                StartCoroutine(SFXCooldown());
                GameObject audioObject = SoundManager.instance.Spawn3DAudio(dropSFX, transform.position);
                Destroy(audioObject, dropSFX.clip.length);
            }
        }
    }

    IEnumerator SFXCooldown()
    {
        yield return new WaitForSeconds(sfxDropDelay);
        canSFX = true;
    }
}
