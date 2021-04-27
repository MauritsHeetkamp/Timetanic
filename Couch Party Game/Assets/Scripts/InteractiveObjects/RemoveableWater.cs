﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoveableWater : MonoBehaviour
{
    public float waterAmount;
    [SerializeField] float maxWaterAmount;
    [SerializeField] float minWaterHeight, maxWaterHeight;
    [SerializeField] float waterRaiseSpeed;

    float targetHeight;

    Coroutine moveWaterRoutine;

    public UnityAction onRemovedWater;
    [SerializeField] UnityEvent onRemovedWaterEvent;

    public bool removed;

    IEnumerator Test()
    {
        while(waterAmount > 0)
        {
            yield return new WaitForSeconds(2);
            ChangeWaterAmount(-1);
        }
    }

    private void Start()
    {
        targetHeight = minWaterHeight + ((waterAmount / maxWaterAmount) * maxWaterHeight - minWaterHeight);
        transform.localPosition = new Vector3(transform.localPosition.x, targetHeight, transform.localPosition.z);
        StartCoroutine(Test());
    }

    public void ChangeWaterAmount(float amount)
    {
        if (!removed)
        {
            Debug.Log("MODIFIED WITH " + amount.ToString());
            waterAmount += amount;

            if (waterAmount > maxWaterAmount)
            {
                waterAmount = maxWaterAmount;
            }

            targetHeight = minWaterHeight + ((waterAmount / maxWaterAmount) * (maxWaterHeight - minWaterHeight));

            if (moveWaterRoutine == null)
            {
                moveWaterRoutine = StartCoroutine(MoveWaterHeight());
            }

            if (waterAmount <= 0)
            {
                waterAmount = 0;
                removed = true;
                if(onRemovedWater != null)
                {
                    onRemovedWater.Invoke();
                }
            }
        }
    }

    public void Reset()
    {
        removed = false;
        waterAmount = maxWaterAmount;
        targetHeight = maxWaterHeight;
        moveWaterRoutine = StartCoroutine(MoveWaterHeight());
    }

    IEnumerator MoveWaterHeight()
    {
        while(transform.localPosition.y != targetHeight)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(transform.localPosition.x, targetHeight, transform.localPosition.z), waterRaiseSpeed);
            yield return null;
        }
        moveWaterRoutine = null;
    }
}
