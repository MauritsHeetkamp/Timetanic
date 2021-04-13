using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Custom.Time;

public class ResultsMenu : MonoBehaviour
{
    UnityAction onCompletedStep;
    Coroutine currentStepRoutine;

    int passengersSaved;
    int passengersDied;
    CountdownTimer timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Reset()
    {
        if(currentStepRoutine != null)
        {
            StopCoroutine(currentStepRoutine);
        }
        onCompletedStep = null;
    }

    void CalculateScore(int _passengersSaved, int _passengersDied, CountdownTimer _timer)
    {
        Reset();

        passengersSaved = _passengersSaved;
        passengersDied = _passengersDied;
        timer = _timer;

        onCompletedStep += CalculateDeadPassengers;
        currentStepRoutine = StartCoroutine(CalculateAlivePassengersRoutine());
    }

    IEnumerator CalculateAlivePassengersRoutine()
    {

    }

    void CalculateDeadPassengers()
    {
        onCompletedStep -= CalculateDeadPassengers;
        onCompletedStep += CalculateRemainingTime;
        currentStepRoutine = StartCoroutine(CalculateDeadPassengersRoutine());
    }

    IEnumerator CalculateDeadPassengersRoutine()
    {

    }

    void CalculateRemainingTime()
    {
        onCompletedStep -= CalculateRemainingTime;
    }

    IEnumerator CalculateRemainingTimeRoutine()
    {

    }
}
