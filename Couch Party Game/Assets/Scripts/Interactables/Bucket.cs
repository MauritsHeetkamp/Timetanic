using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : Extuingisher
{
    [SerializeField] string unfilledName, filledName;
    [SerializeField] Sprite unfilledSprite, filledSprite;

    [SerializeField] Vector3 collsionCheckLocationOffset;
    [SerializeField] float waterCollisionCheckRange;
    [SerializeField] Transform bucketFiller;
    [SerializeField] Vector3 emptyScale, filledScale;
    [SerializeField] Vector3 emptyLocation, filledLocation;
    [SerializeField] float fillSpeed;
    [SerializeField] string fillAnim = "Fill";
    [SerializeField] AnimationClip fillAnimation, emptyAnimation;
    [SerializeField] string emptyAnim = "Empty";
    [SerializeField] LayerMask fillWaterCollisionCheckLayer;
    [SerializeField] LayerMask dropWaterCollisionCheckLayer;

    [SerializeField] AudioClip fillSFX;

    [SerializeField] float filledAmount;
    [SerializeField] float capacity;

    private void Start()
    {
        if(filledAmount <= 0)
        {
            grabbableName = unfilledName;
            grabbableImage = unfilledSprite;
        }
        else
        {
            grabbableName = filledName;
            grabbableImage = filledSprite;
        }
    }

    public override bool CheckUse()
    {
        if (canUse)
        {
            return true;
        }
        return false;
    }

    public override void Use()
    {
        base.Use();

        if(filledAmount > 0)
        {
            StartCoroutine(EmptyBucket());
        }
        else
        {
            StartCoroutine(FillBucket());
        }
    }

    public override void StopUse()
    {
        //Nothing should happen
    }

    IEnumerator FillBucket()
    {
        Debug.Log("FILLING");
        Vector3 targetScale = Vector3.zero;
        Vector3 targetLocation = Vector3.zero;
        currentInteractingPlayer.animator.SetTrigger(fillAnim);

        RaycastHit hitData;

        Vector3 localOffset = collsionCheckLocationOffset.x * currentInteractingPlayer.transform.right;
        localOffset += collsionCheckLocationOffset.y * currentInteractingPlayer.transform.up;
        localOffset += collsionCheckLocationOffset.z * currentInteractingPlayer.transform.forward;
        Ray ray = new Ray(currentInteractingPlayer.transform.position + localOffset, -currentInteractingPlayer.transform.up);

        RemoveableWater removeableWater = null;

        if (Physics.Raycast(ray, out hitData, waterCollisionCheckRange, fillWaterCollisionCheckLayer))
        {
            Debug.Log(hitData.transform.gameObject);
            if (hitData.transform.GetComponent<RemoveableWater>() != null)
            {
                RemoveableWater thisWater = hitData.transform.GetComponent<RemoveableWater>();
                if (thisWater.waterAmount > 0 && !thisWater.removed)
                {
                    removeableWater = thisWater;
                }
            }
        }

        float timePassed = 0;

        if(removeableWater != null && removeableWater.waterAmount > 0)
        {

            float amountToRemove = 0;
            if (removeableWater.waterAmount > capacity)
            {
                amountToRemove = capacity;
            }
            else
            {
                amountToRemove = removeableWater.waterAmount;
            }

            if (amountToRemove != 0)
            {
                if(filledAmount == 0 && !string.IsNullOrEmpty(filledName) && filledSprite != null)
                {
                    ChangeGrabbableData(filledName, filledSprite);
                }
                filledAmount += amountToRemove;
                removeableWater.ChangeWaterAmount(-amountToRemove);
            }

        }

        GameObject fillAudio = null;

        while(timePassed < fillAnimation.length)
        {
            yield return null;
            timePassed += Time.deltaTime;

            if (timePassed > fillAnimation.length)
            {
                timePassed = fillAnimation.length;
            }

            if (removeableWater != null)
            {
                if(fillAudio == null && SoundManager.instance != null)
                {
                    fillAudio = SoundManager.instance.SpawnAudio(fillSFX, false);
                }

                float progress = timePassed / fillAnimation.length;

                Vector3 modifier = filledScale - emptyScale;
                modifier *= filledAmount / capacity;

                targetScale = (emptyScale + modifier) * progress;


                modifier = filledLocation - emptyLocation;
                modifier *= filledAmount / capacity;
                targetLocation = (emptyLocation + modifier) * progress;

                bucketFiller.localPosition = Vector3.MoveTowards(bucketFiller.localPosition, targetLocation, fillSpeed);
                bucketFiller.localScale = Vector3.MoveTowards(bucketFiller.localScale, targetScale, fillSpeed);
            }
        }

        if(fillAudio != null)
        {
            Destroy(fillAudio);
        }

        if (removeableWater != null)
        {
            while (bucketFiller.localPosition != targetLocation || bucketFiller.localScale != targetScale)
            {
                yield return null;
                bucketFiller.localPosition = Vector3.MoveTowards(bucketFiller.localPosition, targetLocation, fillSpeed);
                bucketFiller.localScale = Vector3.MoveTowards(bucketFiller.localScale, targetScale, fillSpeed);
            }
        }
        canUse = true;
    }

    IEnumerator EmptyBucket()
    {
        Debug.Log("EMPTYING");

        if (!string.IsNullOrEmpty(unfilledName) && unfilledSprite != null)
        {
            ChangeGrabbableData(unfilledName, unfilledSprite);
        }

        Vector3 targetScale = Vector3.zero;
        Vector3 targetLocation = Vector3.zero;
        currentInteractingPlayer.animator.SetTrigger(emptyAnim);

        RaycastHit hitData;

        Vector3 localOffset = collsionCheckLocationOffset.x * currentInteractingPlayer.transform.right;
        localOffset += collsionCheckLocationOffset.y * currentInteractingPlayer.transform.up;
        localOffset += collsionCheckLocationOffset.z * currentInteractingPlayer.transform.forward;
        Ray ray = new Ray(currentInteractingPlayer.transform.position + localOffset, -currentInteractingPlayer.transform.up);

        if(Physics.Raycast(ray, out hitData, waterCollisionCheckRange, dropWaterCollisionCheckLayer))
        {
            Debug.Log(hitData.transform.gameObject);
            if(hitData.transform.GetComponent<RemoveableWater>() != null)
            {
                hitData.transform.GetComponent<RemoveableWater>().ChangeWaterAmount(filledAmount);
            }
        }

        float timePassed = 0;



        while (timePassed < emptyAnimation.length)
        {
            yield return null;
            timePassed += Time.deltaTime;

            if (timePassed > emptyAnimation.length)
            {
                timePassed = emptyAnimation.length;
            }

            float progress = 1 - (timePassed / fillAnimation.length);

            Vector3 modifier = filledScale - emptyScale;
            modifier *= filledAmount / capacity;

            targetScale = (emptyScale + modifier) * progress;


            modifier = filledLocation - emptyLocation;
            modifier *= filledAmount / capacity;
            targetLocation = (emptyLocation + modifier) * progress;

            bucketFiller.localPosition = Vector3.MoveTowards(bucketFiller.localPosition, targetLocation, fillSpeed);
            bucketFiller.localScale = Vector3.MoveTowards(bucketFiller.localScale, targetScale, fillSpeed);
        }

        while (bucketFiller.localPosition != targetLocation || bucketFiller.localScale != targetScale)
        {
            yield return null;
            bucketFiller.localPosition = Vector3.MoveTowards(bucketFiller.localPosition, targetLocation, fillSpeed);
            bucketFiller.localScale = Vector3.MoveTowards(bucketFiller.localScale, targetScale, fillSpeed);
        }
        filledAmount = 0;

        canUse = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if(currentInteractingPlayer != null)
        {
            Vector3 localOffset = collsionCheckLocationOffset.x * currentInteractingPlayer.transform.right;
            localOffset += collsionCheckLocationOffset.y * currentInteractingPlayer.transform.up;
            localOffset += collsionCheckLocationOffset.z * currentInteractingPlayer.transform.forward;
            Gizmos.DrawRay(currentInteractingPlayer.transform.position + localOffset, -currentInteractingPlayer.transform.up);
        }
    }
}
