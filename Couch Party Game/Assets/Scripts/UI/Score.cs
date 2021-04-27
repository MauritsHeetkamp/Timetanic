using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Score : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string changedScoreTrigger;
    public int currentScore;

    [SerializeField] TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText.text = currentScore.ToString();
    }

    public void ChangeScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();

        if(animator != null)
        {
            animator.SetTrigger(changedScoreTrigger);
        }
    }
}
