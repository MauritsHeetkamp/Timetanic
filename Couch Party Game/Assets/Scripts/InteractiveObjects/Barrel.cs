using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    float defaultColliderY;
    [SerializeField] Vector3 moveAmount;
    [SerializeField] Transform actualBarrel;

    [SerializeField] float speedMultiplier;
    [SerializeField] float knockbackMultiplier;
    [SerializeField] float upwardsModifier;
    [SerializeField] bool destroyOnCollision;

    [SerializeField] string[] entityTags;

    [SerializeField] CapsuleCollider collider;
    [SerializeField] Vector3 offset;

    [Header("Animations")]
    [SerializeField] Animator barrelAnimator;
    [SerializeField] float minAnimSpeed, maxAnimSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        defaultColliderY = collider.center.y;
        if(barrelAnimator != null)
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
}
