﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom.Audio;


public class Barrel : MonoBehaviour
{
    float defaultColliderY;
    [SerializeField] Vector3 moveAmount;
    [SerializeField] Transform actualBarrel;

    [SerializeField] float speedMultiplier;
    [SerializeField] float knockbackMultiplier;
    [SerializeField] float upwardsModifier;

    [SerializeField] string[] entityTags;

    [SerializeField] CapsuleCollider collider;
    [SerializeField] Vector3 offset;

    [SerializeField] ThreeDAudioPrefab destroySFX;

    [Header("Animations")]
    [SerializeField] Animator barrelAnimator;
    [SerializeField] float minAnimSpeed, maxAnimSpeed;


    public GameObject destroyParticle;
    [SerializeField] float particleDuration = 1f;

    void Awake()
    {
        defaultColliderY = collider.center.y;
        if (barrelAnimator != null)
        {
            barrelAnimator.speed = Random.Range(minAnimSpeed, maxAnimSpeed);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newCenter = collider.center;
        newCenter.y = defaultColliderY + actualBarrel.localPosition.y;
        newCenter += offset;
        collider.center = newCenter;

        transform.Translate(moveAmount * Time.fixedDeltaTime * speedMultiplier);
    }


    private void OnCollisionEnter(Collision collision)
    {
        foreach(string tag in entityTags)
        {
            if(collision.transform.tag == tag)
            {
                Vector3 knockback = transform.forward * knockbackMultiplier;
                knockback.y += upwardsModifier;

                collision.transform.GetComponent<Entity>().Knockback(knockback);

                Destroy(gameObject);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        GameObject newPoofParticle = Instantiate(destroyParticle, actualBarrel.position, Quaternion.identity);

        if(SoundManager.instance != null)
        {
            GameObject destroySound = SoundManager.instance.Spawn3DAudio(destroySFX, actualBarrel.position);
            Destroy(destroySound, destroySFX.clip.length);
        }

        Destroy(newPoofParticle, particleDuration);

        Debug.Log("SPAWNED PARTICLE");
    }
}
