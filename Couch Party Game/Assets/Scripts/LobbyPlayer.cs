using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    public Camera previewCamera;

    [SerializeField] string[] possibleAnimations;
    [SerializeField] Animator animator;


    public void SelectRandomAnim()
    {
        if(animator != null && possibleAnimations.Length > 0)
        {
            animator.Play(possibleAnimations[Random.Range(0, possibleAnimations.Length)]);
        }
    }
}
