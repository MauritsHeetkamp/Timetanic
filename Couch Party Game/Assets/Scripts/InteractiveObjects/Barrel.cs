using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] Vector3 moveAmount;
    [SerializeField] Transform masterObject;

    [SerializeField] float speedMultiplier;
    [SerializeField] float knockbackMultiplier;
    [SerializeField] float upwardsModifier;
    [SerializeField] bool destroyOnCollision;

    [SerializeField] string[] entityTags;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        masterObject.Translate(moveAmount * Time.fixedDeltaTime * speedMultiplier);
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
