using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MovingEntity
{
    [Header("General")]
    public Role role; //The role of the player, this is used for where the player needs to be spawned
    public List<MobilePassenger> followingPassengers = new List<MobilePassenger>();

    [Header("Movement")]
    [SerializeField]Vector2 rawMovement; //The raw input vector (New InputSystem)

    public bool canMove = true;
    [SerializeField] float maxVelocity = 1; //The maximum velocity while moving
    public int decellerationBlocks; //System that blocks decelleration when higher then 0
    [SerializeField] float decellerationSpeed = 1;
    [SerializeField] float rotateSpeed = 1;

    [Header("Jumping")]
    [SerializeField] float jumpVelocity = 1;
    [SerializeField] bool canJump = true;

    [SerializeField] float jumpDetectionRange = 1; //The range downwards to see if the player is grounded
    [SerializeField] LayerMask jumpableLayers; //The layers that the player can jump on

    [Header("Slopes")]

    [SerializeField] float maxAngle; //The largest angle the player can climb
    [SerializeField] float slopeBoosterModifier = 1; //The multiplier for how much you get boosted up on slopes
    [SerializeField] Transform slopeCheckOrigin; //Location from where the rays are cast to check slopes
    [SerializeField] float forwardRayRange, downwardRayRange; // The range for the rays to check slopes
    [SerializeField] LayerMask slopeMask; //The layers that can be treated as slopes
    public bool onSlope;

    [Header("Camera")]
    public Transform playerCamera;
    [SerializeField] bool followX = true, followY = true, followZ = true; //Directions the camera should be following
    public float zDistance, yDistance; //Camera distance from the player away
    [SerializeField] float followSpeed;

    public GameObject attachedSplitscreen; //Split screen owned by this player

    [Header("Interaction")]
    [SerializeField] Transform interactionBox; //Box that checks if an interactable is inside
    [SerializeField] LayerMask interactableLayers; 
    Interactable nearestInteractable;
    [SerializeField] float interactDelay; //Delay before u can interact again
    [SerializeField] bool canInteract = true;
    public Interactable currentUsingInteractable; //The current interactable the player is using
    public Grabbable currentHoldingItem; //The current item the player is holding

    [Header("Throwing")]
    [SerializeField] float throwVelocity, throwHeight;
    [SerializeField] float minForwardRotationVelocity, maxForwardRotationVelocity;
    [SerializeField] float minSidewaysRotationVelocity, maxSidewaysRotationVelocity;

    [Header("Dashing")]
    bool canDash = true;
    [SerializeField] float dashVelocity; // The force of the dash
    [SerializeField] float dashCooldown; // Minimal duration before you can dash again
    [SerializeField] bool stopOnCollision = true; //Player can't move until he is in no motion anymore
    [SerializeField] float checkRange; // The range that the ray checks for collision
    [SerializeField] float dashDuration;
    [SerializeField] LayerMask hittableLayers; // Layers that cancels the dash

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
        zDistance = zDistance == 0 ? playerCamera.localPosition.z : zDistance; //Sets the zDistance to the prefabs location if the distance is 0
        yDistance = yDistance == 0 ? playerCamera.localPosition.y : yDistance; //Sets the yDistance to the prefabs location if the distance is 0
        if(playerCamera != null)
        {
            playerCamera.parent = null; //Disattaches the players camera for free movement
        }
    }

    // Uses holding item if possible
    public void UseCurrentItem(InputAction.CallbackContext context)
    {
        if (canInteract && currentHoldingItem != null) //Checks if you can interact and have an equipped item
        {
            UsableGrabbable grabbable = currentHoldingItem.GetComponent<UsableGrabbable>();
            if(grabbable != null) //Checks if the item is an usable grabbable
            {
                if (context.started && grabbable.CheckUse()) //Checks input and if the grabbable can be used
                {
                    grabbable.Use(); // Uses the item
                }
                if (context.canceled) //Checks input
                {
                    grabbable.StopUse(); // Stops using the item
                }
            }

        }
    }

    // Throws current holding item
    public void ThrowCurrentItem(InputAction.CallbackContext context)
    {
        if(context.started && canInteract && currentHoldingItem != null) //Checks input, if you can interact and if you are holding an item
        {
            Grabbable lastItem = currentHoldingItem; // The item you were holding
            currentHoldingItem.Disattach();
            Vector3 throwDirection = transform.forward; // The direction the item will be thrown towards
            throwDirection = throwDirection.normalized * throwVelocity;
            throwDirection.y = throwHeight;

            Vector3 rotationDirection = new Vector3(-transform.forward.z, 0, -transform.forward.x); // The rotating velocity the object will have
            lastItem.rigid.AddRelativeTorque(new Vector3(Random.Range(minForwardRotationVelocity, maxForwardRotationVelocity), 0 , Random.Range(minSidewaysRotationVelocity, maxSidewaysRotationVelocity))); // Adds the rotating velocity
            lastItem.rigid.AddForce(throwDirection); // Adds the velocity towards the target direction
        }
    }

    // Drops the current holding item
    public void DropCurrentItem(InputAction.CallbackContext context)
    {
        if (context.started && currentHoldingItem != null) // Checks input and if you are holding an item
        {
            currentHoldingItem.Disattach();
        }
    }

    // Handles jumping
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.started && canJump) // Checks input and if player can jump
        {
            if (Physics.Raycast(transform.position, -transform.up, jumpDetectionRange, jumpableLayers, QueryTriggerInteraction.Ignore)) // Checks if grounded (for jumps)
            {
                thisRigid.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse); // Adds jump force
            }
        }
    }

    // Sets the movement amounts
    public void SetMoveAmount(InputAction.CallbackContext context)
    {
        if(context.performed || context.started) // Checks input
        {
            rawMovement = context.ReadValue<Vector2>(); // Sets raw movement vector
        }
        else // Stopped moving
        {
            rawMovement = Vector3.zero; // Resets raw movement vector
        }
    }

    // Handles slopes
    void CheckSlope()
    {
        RaycastHit forwardHitdata, downwardHitdata; // Hit data for the forward and downward ray

        if (Physics.Raycast(slopeCheckOrigin.position, new Vector3(rawMovement.x, 0, rawMovement.y), out forwardHitdata, forwardRayRange, slopeMask)) // Checks if a slope is in front of your check origin
        {
            if (Physics.Raycast(slopeCheckOrigin.position, new Vector3(rawMovement.x, 0, rawMovement.y), out downwardHitdata, downwardRayRange, slopeMask)) //Checks if a slope is underneath your check origin
            {
                float angle = Vector3.Angle(transform.up, forwardHitdata.transform.up); // The angle of the slope
                if (angle <= maxAngle)
                {
                    if (!onSlope)
                    {
                        onSlope = true;
                        thisRigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; // Sets constraints to ignore gravity
                    }
                    float yDifference = forwardHitdata.point.y - transform.position.y; // Height difference
                    transform.Translate(new Vector3(0, yDifference * slopeBoosterModifier, 0)); // Boost upwards based on height difference
                }
                else // Slope angle is too high
                {
                    if (onSlope)
                    {
                        onSlope = false;
                        thisRigid.constraints = RigidbodyConstraints.FreezeRotation; // Sets constraints to use gravity
                    }
                }
            }
            else // No slope underneath check origin
            {
                if (onSlope)
                {
                    onSlope = false;
                    thisRigid.constraints = RigidbodyConstraints.FreezeRotation; // Sets constraints to use gravity
                }
            }
        }
        else // No slope in front of check origin
        {
            if (onSlope)
            {
                onSlope = false;
                thisRigid.constraints = RigidbodyConstraints.FreezeRotation; // Sets constraints to use gravity
            }
        }
    }

    // Handles player movement
    void Movement()
    {
        Vector3 movementAmount = new Vector3(rawMovement.x, 0, rawMovement.y);

        movementAmount = movementAmount.normalized;

        if (rawMovement.x != 0 || rawMovement.y != 0) // If not standing still
        {

            movementAmount = movementAmount * movementSpeed * Time.fixedDeltaTime;

            Vector3 newVelocity = thisRigid.velocity + movementAmount; // Calculates new velocity

            Vector3 actualMaxVelocity = new Vector3(maxVelocity, maxVelocity, maxVelocity); // Creates the max velocity vector

            if (newVelocity.x < -actualMaxVelocity.x)
            {
                newVelocity.x = -actualMaxVelocity.x; // Sets velocity.x if its under the min velocity;
            }
            if (newVelocity.x > actualMaxVelocity.x)
            {
                newVelocity.x = actualMaxVelocity.x; // Sets velocity.x if its above the max velocity;
            }

            if (newVelocity.z < -actualMaxVelocity.z)
            {
                newVelocity.z = -actualMaxVelocity.z; // Sets velocity.z if its under the min velocity;
            }
            if (newVelocity.z > actualMaxVelocity.z)
            {
                newVelocity.z = actualMaxVelocity.z; // Sets velocity.z if its above the max velocity;
            }

            newVelocity = newVelocity - thisRigid.velocity; // Removes current velocity to get the force to add

            thisRigid.AddForce(newVelocity, ForceMode.Acceleration); // Adds the movement velocity

            Vector3 velocityDirection = thisRigid.velocity;
            velocityDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(movementAmount.normalized); // Calculates the direction the player should be facing
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime); // Rotates the player towards the target rotation
        }
        else // If not moving
        {
            if (decellerationBlocks <= 0) // Checks if decelleration should be used
            {
                if (thisRigid.velocity.x < -0.1 || thisRigid.velocity.x > 0.1f || thisRigid.velocity.z < -0.1 || thisRigid.velocity.z > 0.1f) // Checks if decelleration is necessary
                {
                    Vector3 velocityDecrease = Vector3.zero;

                    if (thisRigid.velocity.x > 0)
                    {
                        velocityDecrease.x = decellerationSpeed * Time.fixedDeltaTime > thisRigid.velocity.x ? thisRigid.velocity.x : decellerationSpeed * Time.fixedDeltaTime; // Sets velocity decrease amount of the x axis
                    }
                    if (thisRigid.velocity.z > 0)
                    {
                        velocityDecrease.z = decellerationSpeed * Time.fixedDeltaTime > thisRigid.velocity.z ? thisRigid.velocity.z : decellerationSpeed * Time.fixedDeltaTime; // Sets velocity decrease amount of the z axis
                    }

                    if (thisRigid.velocity.x < 0)
                    {
                        velocityDecrease.x = -decellerationSpeed * Time.fixedDeltaTime < thisRigid.velocity.x ? thisRigid.velocity.x : -decellerationSpeed * Time.fixedDeltaTime; // Sets velocity decrease amount of the x axis
                    }
                    if (thisRigid.velocity.z < 0)
                    {
                        velocityDecrease.z = -decellerationSpeed * Time.fixedDeltaTime < thisRigid.velocity.z ? thisRigid.velocity.z : -decellerationSpeed * Time.fixedDeltaTime; // Sets velocity decrease amount of the z axis
                    }

                    thisRigid.velocity -= velocityDecrease; // Decellerates the player with the set amount
                }
            }
        }
    }

    // Checks to use a dash
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && canDash) // Checks input and if the player can dash
        {
            canMove = false;
            canDash = false;
            StartCoroutine(DashRoutine()); // Performs dash
        }
    }

    // Handles dash
    IEnumerator DashRoutine()
    {
        float duration = 0; // duration passed since start of dash

        while (duration < dashDuration)
        {
            float decreaseOverDuration = (dashDuration - duration) / dashDuration; // How much of the duration is left (range from 0 to 1)
            RaycastHit hitData;

            if(Physics.Raycast(transform.position, transform.forward, out hitData, checkRange, hittableLayers, QueryTriggerInteraction.Ignore)) // Checks for collision during the dash
            {
                if (stopOnCollision) // Should the dash stop whenever the player collides
                {
                    break;
                }
            }
            transform.Translate(Vector3.forward * dashVelocity * Time.deltaTime * decreaseOverDuration); // Moves the player
            yield return null;
            duration += Time.deltaTime;
        }

        StartCoroutine(DashCooldown()); // Starts the cooldown of the dash
    }

    // Cooldown of the dash
    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
        canMove = true;
    }

    // Checks if the player is near something that they can interact with
    void CheckInteract()
    {
        Collider[] hitColliders = Physics.OverlapBox(interactionBox.position, interactionBox.lossyScale / 2, interactionBox.rotation, interactableLayers); // Checks for interactables in range

        Interactable closestInteractable = null;

        if (hitColliders.Length > 0) // Are there any interactables nearby?
        {
            float closestDistance = float.MaxValue;

            foreach (Collider col in hitColliders)
            {
                Interactable interactable = col.GetComponent<Interactable>();
                if (interactable.CanInteract(this)) // Checks if the interactable can be used
                {
                    float distance = Vector3.Distance(col.transform.position, transform.position);
                    if (distance < closestDistance || closestInteractable == null) // Checks if this interactable is closer then the current closest one
                    {
                        closestInteractable = interactable;
                        closestDistance = distance;
                    }
                }
            }
        }

        nearestInteractable = closestInteractable; // Sets the nearest interactable
    }

    // Handles interaction
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && nearestInteractable != null && canInteract) // Checks input, if an interactable is near and if you can interact
        {
            nearestInteractable.Interact(this);
        }

    }

    // Whenever the player has completed interaction
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

    // Delay before the player can interact again
    IEnumerator InteractDelay()
    {
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;
    }

    // Resets the local camera location
    public void ResetCameraLocation()
    {
        Vector3 targetPosition = playerCamera.transform.position;
        if (followX)
        {
            targetPosition.x = transform.position.x; // Calculates the target location on the x axis
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance;
            targetPosition.y = transform.position.y + distanceToMoveOnY; // Calculates the target location on the y axis
        }
        if (followZ)
        {
            float distanceToMoveOnZ = playerCamera.transform.position.z - transform.position.z <= 0 ? -1 * zDistance : 1 * zDistance;
            targetPosition.z = transform.position.z + distanceToMoveOnZ; // Calculates the target location on the z axis
        }
        playerCamera.transform.position = targetPosition; // Sets camera location
    }

    // Handles the camera movement
    void CameraFollow()
    {
        Vector3 targetPosition = playerCamera.transform.position;
        if (followX)
        {
            targetPosition.x = transform.position.x; // Calculates the target location on the x axis
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance;
            targetPosition.y = transform.position.y + distanceToMoveOnY; // Calculates the target location on the y axis
        }
        if (followZ)
        {
            float distanceToMoveOnZ = playerCamera.transform.position.z - transform.position.z <= 0 ? -1 * zDistance : 1 * zDistance;
            targetPosition.z = transform.position.z + distanceToMoveOnZ; // Calculates the target location on the z axis
        }
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, followSpeed * Time.deltaTime); // Smoothly goes to the target location
    }

    public enum Role { TestRole, OtherTestRole} // Roles for spawn locations
}
