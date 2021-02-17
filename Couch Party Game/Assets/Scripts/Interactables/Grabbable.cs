using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    public Rigidbody rigid;
    public string itemName;
    [SerializeField] Vector3 itemLocalPosition, itemLocalEulers;
    [SerializeField] Transform leftHandle, rightHandle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CompleteInteract()
    {
        currentInteractingPlayer.currentUsingInteractable = null;
        Attach();
        currentInteractingPlayer.FinishedInteract();
    }

    public virtual void Attach()
    {
        rigid.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        currentInteractingPlayer.currentHoldingItem = this;
        transform.parent = currentInteractingPlayer.transform;
        transform.localPosition = itemLocalPosition;
        transform.localEulerAngles = itemLocalEulers;
    }


    public virtual void Disattach()
    {
        transform.parent = null;
        rigid.isKinematic = false;
        currentInteractingPlayer.currentHoldingItem = null;
        GetComponent<Collider>().enabled = true;
        currentInteractingPlayer.FinishedInteract();
        InteractDelay();
        currentInteractingPlayer = null;
    }

    public virtual void Break()
    {
        currentInteractingPlayer.currentHoldingItem = null;
        currentInteractingPlayer.FinishedInteract();
    }
}
