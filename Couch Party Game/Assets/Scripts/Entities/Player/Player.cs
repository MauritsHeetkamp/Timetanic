using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MovingEntity
{
    [Header("General")]
    public Role role; //The role of the player, this is used for where the player needs to be spawned
    PlayerControls controls;

    [Header("Movement")]
    [SerializeField]Vector2 rawMovement;

    public bool canMove = true;
    [SerializeField] float maxVelocity = 1;
    public int decellerationBlocks;
    [SerializeField] float decellerationSpeed = 1;
    [SerializeField] float rotateSpeed = 1;

    [Header("Jumping")]
    [SerializeField] float jumpVelocity = 1;
    [SerializeField] bool canJump = true;
    [SerializeField] string jumpButton;

    [SerializeField] float jumpDetectionRange = 1;
    [SerializeField] LayerMask jumpableLayers;

    [Header("Slopes")]

    [SerializeField] float maxAngle;
    [SerializeField] float slopeBoosterModifier = 1;
    [SerializeField] Transform slopeCheckOrigin;
    [SerializeField] float forwardRayRange, downwardRayRange;
    [SerializeField] LayerMask slopeMask;
    public bool onSlope;

    [Header("Camera")]
    public Transform playerCamera;
    [SerializeField] bool followX = true, followY = true, followZ = true;
    public float zDistance, yDistance;
    [SerializeField] float followSpeed;

    public GameObject attachedSplitscreen;

    [Header("Interaction")]
    [SerializeField] string interactButton;
    [SerializeField] Transform interactionBox; //Box that checks if an interactable is inside
    [SerializeField] LayerMask interactableLayers;
    Interactable nearestInteractable;
    [SerializeField] float interactDelay; //Delay before u can interact again
    [SerializeField] bool canInteract = true;
    public Interactable currentUsingInteractable; //The current interactable the player is using or holding
    public Grabbable currentHoldingItem; //The current item the player is holding;

    [SerializeField] string dropButton;

    [Header("Throwing")]
    [SerializeField] string throwButton;
    [SerializeField] float throwVelocity, throwHeight;
    [SerializeField] float minForwardRotationVelocity, maxForwardRotationVelocity;
    [SerializeField] float minSidewaysRotationVelocity, maxSidewaysRotationVelocity;

    [Header("Dashing")]
    [SerializeField] string dashButton;
    bool canDash = true;
    [SerializeField] float dashVelocity;
    [SerializeField] float dashCooldown; // Minimal duration before you can dash again
    [SerializeField] bool stopOnCollision = true; //Player can't move until he is in no motion anymore
    [SerializeField] float checkRange;
    [SerializeField] float dashDuration;
    [SerializeField] LayerMask hittableLayers;

    private void Update()
    {
        CameraFollow();
        CheckInteract();
    }

    private void FixedUpdate()
    {
        Movement();
        CheckSlope();
    }


    private void Start()
    {
        zDistance = zDistance == 0 ? playerCamera.localPosition.z : zDistance;
        yDistance = yDistance == 0 ? playerCamera.localPosition.y : yDistance;
        if(playerCamera != null)
        {
            playerCamera.parent = null;
        }
    }

    public void UseCurrentItem(InputAction.CallbackContext context)
    {
        if (canInteract && currentHoldingItem != null)
        {
            UsableGrabbable grabbable = currentHoldingItem.GetComponent<UsableGrabbable>();
            if(grabbable != null)
            {
                if (context.started && grabbable.CheckUse())
                {
                    if (grabbable.CheckUse())
                    {
                        grabbable.Use();
                    }
                }
                if (context.canceled)
                {
                    grabbable.StopUse();
                }
            }

        }
    }

    public void ThrowCurrentItem(InputAction.CallbackContext context)
    {
        if(context.started && canInteract && currentHoldingItem != null)
        {
            Grabbable lastItem = currentHoldingItem;
            currentHoldingItem.Disattach();
            Vector3 throwDirection = transform.forward;
            throwDirection = throwDirection.normalized * throwVelocity;
            throwDirection.y = throwHeight;

            Vector3 rotationDirection = new Vector3(-transform.forward.z, 0, -transform.forward.x);
            Debug.Log(rotationDirection.normalized);
            lastItem.rigid.AddRelativeTorque(new Vector3(Random.Range(minForwardRotationVelocity, maxForwardRotationVelocity), 0 , Random.Range(minSidewaysRotationVelocity, maxSidewaysRotationVelocity)));
            lastItem.rigid.AddForce(throwDirection);
        }
    }

    public void DropCurrentItem(InputAction.CallbackContext context)
    {
        if (context.started && currentHoldingItem != null)
        {
            currentHoldingItem.Disattach();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.started && canJump)
        {
            if (Physics.Raycast(transform.position, -transform.up, jumpDetectionRange, jumpableLayers, QueryTriggerInteraction.Ignore))
            {
                thisRigid.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
            }
        }
    }

    public void SetMoveAmount(InputAction.CallbackContext context)
    {
        if(context.performed || context.started)
        {
            rawMovement = context.ReadValue<Vector2>();
        }
        else
        {
            rawMovement = Vector3.zero;
        }
    }

    void CheckSlope()
    {
        RaycastHit forwardHitdata, downwardHitdata;

        if (Physics.Raycast(slopeCheckOrigin.position, new Vector3(rawMovement.x, 0, rawMovement.y), out forwardHitdata, forwardRayRange, slopeMask))
        {
            if (Physics.Raycast(slopeCheckOrigin.position, new Vector3(rawMovement.x, 0, rawMovement.y), out downwardHitdata, downwardRayRange, slopeMask))
            {
                float angle = Vector3.Angle(transform.up, forwardHitdata.transform.up);
                if (angle <= maxAngle)
                {
                    if (!onSlope)
                    {
                        onSlope = true;
                        thisRigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                    }
                    float yDifference = forwardHitdata.point.y - transform.position.y;
                    transform.Translate(new Vector3(0, yDifference * slopeBoosterModifier, 0));
                }
                else
                {
                    if (onSlope)
                    {
                        onSlope = false;
                        thisRigid.constraints = RigidbodyConstraints.FreezeRotation;
                    }
                }
            }
            else
            {
                if (onSlope)
                {
                    onSlope = false;
                    thisRigid.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
        else
        {
            if (onSlope)
            {
                onSlope = false;
                thisRigid.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void Movement()
    {
        Vector3 movementAmount = new Vector3(rawMovement.x, 0, rawMovement.y);

        movementAmount = movementAmount.normalized;

        if (rawMovement.x != 0 || rawMovement.y != 0)
        {

            movementAmount = movementAmount * movementSpeed * Time.fixedDeltaTime;

            Vector3 newVelocity = thisRigid.velocity + movementAmount;

            Vector3 actualMaxVelocity = newVelocity.normalized;
            actualMaxVelocity *= maxVelocity;

            if (newVelocity.x < -actualMaxVelocity.x)
            {
                newVelocity.x = -actualMaxVelocity.x;
            }
            if (newVelocity.x > actualMaxVelocity.x)
            {
                newVelocity.x = actualMaxVelocity.x;
            }

            if (newVelocity.z < -actualMaxVelocity.z)
            {
                newVelocity.z = -actualMaxVelocity.z;
            }
            if (newVelocity.z > actualMaxVelocity.z)
            {
                newVelocity.z = actualMaxVelocity.z;
            }

            newVelocity = newVelocity - thisRigid.velocity;

            thisRigid.AddForce(newVelocity, ForceMode.Acceleration);

            Vector3 velocityDirection = thisRigid.velocity;
            velocityDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(movementAmount.normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if (decellerationBlocks <= 0)
            {
                if (thisRigid.velocity.x < -0.1 || thisRigid.velocity.x > 0.1f || thisRigid.velocity.z < -0.1 || thisRigid.velocity.z > 0.1f)
                {
                    Vector3 velocityDecrease = Vector3.zero;

                    if (thisRigid.velocity.x > 0)
                    {
                        velocityDecrease.x = decellerationSpeed * Time.fixedDeltaTime > thisRigid.velocity.x ? thisRigid.velocity.x : decellerationSpeed * Time.fixedDeltaTime;
                    }
                    if (thisRigid.velocity.z > 0)
                    {
                        velocityDecrease.z = decellerationSpeed * Time.fixedDeltaTime > thisRigid.velocity.z ? thisRigid.velocity.z : decellerationSpeed * Time.fixedDeltaTime;
                    }

                    if (thisRigid.velocity.x < 0)
                    {
                        velocityDecrease.x = -decellerationSpeed * Time.fixedDeltaTime < thisRigid.velocity.x ? thisRigid.velocity.x : -decellerationSpeed * Time.fixedDeltaTime;
                    }
                    if (thisRigid.velocity.z < 0)
                    {
                        velocityDecrease.z = -decellerationSpeed * Time.fixedDeltaTime < thisRigid.velocity.z ? thisRigid.velocity.z : -decellerationSpeed * Time.fixedDeltaTime;
                    }

                    thisRigid.velocity -= velocityDecrease;
                }
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && canDash)
        {
            canMove = false;
            canDash = false;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        float duration = 0;

        while (duration < dashDuration)
        {
            float decreaseOverDuration = (dashDuration - duration) / dashDuration;
            RaycastHit hitData;

            if(Physics.Raycast(transform.position, transform.forward, out hitData, checkRange, hittableLayers, QueryTriggerInteraction.Ignore))
            {
                if (stopOnCollision)
                {
                    break;
                }
            }
            transform.Translate(Vector3.forward * dashVelocity * Time.deltaTime * decreaseOverDuration);
            yield return null;
            duration += Time.deltaTime;
        }

        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
        canMove = true;
    }

    void CheckInteract() //Checks if the player is near something that they can interact with
    {
        Collider[] hitColliders = Physics.OverlapBox(interactionBox.position, interactionBox.lossyScale / 2, interactionBox.rotation, interactableLayers);

        Interactable closestInteractable = null;

        if (hitColliders.Length > 0)
        {
            float closestDistance = float.MaxValue;

            foreach (Collider col in hitColliders)
            {
                Interactable interactable = col.GetComponent<Interactable>();
                if (interactable.CanInteract(this))
                {
                    float distance = Vector3.Distance(col.transform.position, transform.position);
                    if (distance < closestDistance || closestInteractable == null)
                    {
                        closestInteractable = interactable;
                        closestDistance = distance;
                    }
                }
            }
        }

        nearestInteractable = closestInteractable;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && nearestInteractable != null && canInteract)
        {
            nearestInteractable.Interact(this);
        }

    }

    public void FinishedInteract()
    {
        if(interactDelay > 0)
        {
            StartCoroutine(InteractDelay());
        }
        else
        {
            canInteract = true;
        }
    }

    IEnumerator InteractDelay()
    {
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;
    }

    public void ResetCameraLocation()
    {
        Vector3 targetPosition = playerCamera.transform.position;
        if (followX)
        {
            targetPosition.x = transform.position.x;
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance;
            targetPosition.y = transform.position.y + distanceToMoveOnY;
        }
        if (followZ)
        {
            float distanceToMoveOnZ = playerCamera.transform.position.z - transform.position.z <= 0 ? -1 * zDistance : 1 * zDistance;
            targetPosition.z = transform.position.z + distanceToMoveOnZ;
        }
        playerCamera.transform.position = targetPosition;
    }

    void CameraFollow()
    {
        Vector3 targetPosition = playerCamera.transform.position;
        if (followX)
        {
            targetPosition.x = transform.position.x;
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance;
            targetPosition.y = transform.position.y + distanceToMoveOnY;
        }
        if (followZ)
        {
            float distanceToMoveOnZ = playerCamera.transform.position.z - transform.position.z <= 0 ? -1 * zDistance : 1 * zDistance;
            targetPosition.z = transform.position.z + distanceToMoveOnZ;
        }
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public enum Role { TestRole, OtherTestRole}
}
