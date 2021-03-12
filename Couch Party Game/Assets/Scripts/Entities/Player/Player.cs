﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MovingEntity
{
    [Header("General")]
    public PlayerData owner;
    [SerializeField] string characterControlScheme = "Player";
    public Role role; //The role of the player, this is used for where the player needs to be spawned
    public List<MobilePassenger> followingPassengers = new List<MobilePassenger>();

    [Header("Animation")]
    public Animator playerAnimator;

    [Header("Movement")]
    [SerializeField]Vector2 rawMovement; //The raw input vector (New InputSystem)

    public int moveBlocks;
    public int decellerationBlocks; //System that blocks decelleration when higher then 0
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
    [SerializeField] bool followXY = true, followY = true; //Directions the camera should be following
    public float zDistance, yDistance; //Camera distance from the player away
    [SerializeField] float followSpeed;
    [SerializeField] Transform cameraDirection; // Handles the camera its direction

    CameraHandler cameraHandler; // Object that manages the cameras

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

    // Handles camera following and interaction checks
    private void Update()
    {
        if(disables <= 0)
        {
            CheckInteract();
        }
    }

    // Handles movement and slope checks
    private void FixedUpdate()
    {
        if (disables <= 0)
        {
            Movement();
            CheckSlope();
        }
        CameraFollow();
    }

    public void TogglePlayer()
    {

    }

    public override void Disable(bool disable)
    {
        base.Disable(disable);

        if(disables > 0)
        {
            if (owner != null) // Is this owner by a player?
            {
                // Assigns buttons their functionality
                owner.onMove -= SetMoveAmount;
                owner.onUse -= UseCurrentItem;
                owner.onThrow -= ThrowCurrentItem;
                owner.onDrop -= DropCurrentItem;
                owner.onJump -= Jump;
                owner.onDash -= Dash;
                owner.onInteract -= Interact;
            }
        }
        else
        {
            if (owner != null) // Is this owner by a player?
            {
                // Assigns buttons their functionality
                owner.onMove += SetMoveAmount;
                owner.onUse += UseCurrentItem;
                owner.onThrow += ThrowCurrentItem;
                owner.onDrop += DropCurrentItem;
                owner.onJump += Jump;
                owner.onDash += Dash;
                owner.onInteract += Interact;
            }
        }
    }

    // Sets input events
    private void OnEnable()
    {
        if(owner != null) // Is this owner by a player?
        {
            // Assigns buttons their functionality
            owner.onMove += SetMoveAmount;
            owner.onUse += UseCurrentItem;
            owner.onThrow += ThrowCurrentItem;
            owner.onDrop += DropCurrentItem;
            owner.onJump += Jump;
            owner.onDash += Dash;
            owner.onInteract += Interact;
        }
    }

    // Removes input events
    private void OnDisable()
    {
        if (owner != null) // Is this owner by a player?
        {
            // Assigns buttons their functionality
            owner.onMove -= SetMoveAmount;
            owner.onUse -= UseCurrentItem;
            owner.onThrow -= ThrowCurrentItem;
            owner.onDrop -= DropCurrentItem;
            owner.onJump -= Jump;
            owner.onDash -= Dash;
            owner.onInteract -= Interact;
        }
    }

    // Unparents the camera for free movement
    private void Awake()
    {
        cameraDirection.parent = null;
    }

    // Start is called before the first frame update
    private void Start()
    {
        OnEnable();
        owner.SwapInputScheme(characterControlScheme); // Sets the control scheme to player controls

        zDistance = zDistance == 0 ? playerCamera.localPosition.z : zDistance; //Sets the zDistance to the prefabs location if the distance is 0
        yDistance = yDistance == 0 ? playerCamera.localPosition.y : yDistance; //Sets the yDistance to the prefabs location if the distance is 0
        if(playerCamera != null)
        {
            playerCamera.parent = null; //Disattaches the players camera for free movement
        }

        if(cameraHandler == null)
        {
            GameObject cameraHandlerObject = GameObject.FindGameObjectWithTag("MainCamera");

            if(cameraHandlerObject != null)
            {
                cameraHandler = cameraHandlerObject.GetComponent<CameraHandler>();
            }
        }

    }

    // Uses holding item if possible
    public void UseCurrentItem(InputAction.CallbackContext context, PlayerData owner)
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
    public void ThrowCurrentItem(InputAction.CallbackContext context, PlayerData owner)
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
    public void DropCurrentItem(InputAction.CallbackContext context, PlayerData owner)
    {
        if (context.started && currentHoldingItem != null) // Checks input and if you are holding an item
        {
            currentHoldingItem.Disattach();
        }
    }

    // Handles jumping
    public void Jump(InputAction.CallbackContext context, PlayerData owner)
    {
        if(context.started && canJump && thisRigid.velocity.y <= 0) // Checks input and if player can jump
        {
            if (Physics.Raycast(transform.position, -transform.up, jumpDetectionRange, jumpableLayers, QueryTriggerInteraction.Ignore)) // Checks if grounded (for jumps)
            {
                thisRigid.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse); // Adds jump force
            }
        }
    }

    // Sets the movement amounts
    public void SetMoveAmount(InputAction.CallbackContext context, PlayerData owner)
    {
        if(context.performed || context.started) // Checks input
        {
            Transform selectedCamera = playerCamera;

            if(cameraHandler != null && !cameraHandler.isSplit && cameraHandler.globalCamera != null)
            {
                selectedCamera = cameraHandler.globalCamera;
            }

            Vector2 axisValue = context.ReadValue<Vector2>();
            Vector3 movementDirection = axisValue.x * selectedCamera.right;
            movementDirection += axisValue.y * selectedCamera.forward;
            movementDirection.y = 0;

            rawMovement = new Vector2(movementDirection.normalized.x, movementDirection.normalized.z); // Sets raw movement vector

        }
        else // Stopped moving
        {
            rawMovement = Vector2.zero; // Resets raw movement vector
        }
    }

    // Handles slopes
    void CheckSlope()
    {
        RaycastHit forwardHitdata, downwardHitdata; // Hit data for the forward and downward ray

        if (Physics.Raycast(slopeCheckOrigin.position, -slopeCheckOrigin.up, out downwardHitdata, downwardRayRange, slopeMask, QueryTriggerInteraction.Ignore) || Physics.Raycast(slopeCheckOrigin.position, -slopeCheckOrigin.up, out downwardHitdata, downwardRayRange, slopeMask, QueryTriggerInteraction.Ignore)) //Checks if a slope is underneath your check origin
        {
            if (!onSlope)
            {
                onSlope = true;
                thisRigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation; // Sets constraints to ignore gravity
                thisRigid.isKinematic = true;
            }
        }
        else
        {
            if (onSlope)
            {
                onSlope = false;
                thisRigid.constraints = RigidbodyConstraints.FreezeRotation; // Sets constraints to use gravity
                thisRigid.isKinematic = false;
            }
        }

        if (Physics.Raycast(slopeCheckOrigin.position, new Vector3(rawMovement.x, 0, rawMovement.y), out forwardHitdata, forwardRayRange, slopeMask, QueryTriggerInteraction.Ignore)) // Checks if a slope is in front of your check origin
        {
            if (onSlope)
            {
                float angle = Vector3.Angle(transform.up, forwardHitdata.transform.up); // The angle of the slope
                if (angle <= maxAngle)
                {
                    float yDifference = forwardHitdata.point.y - transform.position.y; // Height difference
                    transform.Translate(new Vector3(0, yDifference * slopeBoosterModifier, 0)); // Boost upwards based on height difference
                }
            }
        }
    }

    // Handles player movement
    void Movement()
    {
        if(moveBlocks <= 0)
        {
            Vector3 movementAmount = new Vector3(rawMovement.x, 0, rawMovement.y);

            if (movementAmount.x != 0 || movementAmount.z != 0) // If not standing still
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("Walking", true);
                }
                movementAmount = movementAmount * movementSpeed * Time.fixedDeltaTime;

                transform.Translate(movementAmount, Space.World);
                Quaternion targetRotation = Quaternion.LookRotation(movementAmount); // Calculates the direction the player should be facing
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime); // Rotates the player towards the target rotation
            }
            else
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("Walking", false);
                }
            }
        }
    }

    // Checks to use a dash
    public void Dash(InputAction.CallbackContext context, PlayerData owner)
    {
        if (context.started && canDash) // Checks input and if the player can dash
        {
            moveBlocks++;
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
        moveBlocks--;
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
    public void Interact(InputAction.CallbackContext context, PlayerData owner)
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
        if (followXY)
        {
            targetPosition.x = transform.position.x; // Calculates the target location on the x axis
            targetPosition.z = transform.position.z; // Calculates the target location on the z axis

            Vector3 moveBackwardsAmount = zDistance * -cameraDirection.forward; // Calculates how much the camera should be moved backwards (locally)
            moveBackwardsAmount.y = 0;

            targetPosition += moveBackwardsAmount; // Updates the cameras target position with the backwards amount
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance; // Calculates how much the camera should be moved upwards
            targetPosition.y = transform.position.y + distanceToMoveOnY; // Calculates the target location on the y axis
        }
        playerCamera.transform.position = targetPosition; // Sets camera location
    }

    // Handles the camera movement
    void CameraFollow()
    {
        Vector3 targetPosition = playerCamera.transform.position;
        if (followXY)
        {
            targetPosition.x = transform.position.x; // Calculates the target location on the x axis
            targetPosition.z = transform.position.z; // Calculates the target location on the z axis

            Vector3 moveBackwardsAmount = zDistance * -cameraDirection.forward;
            moveBackwardsAmount.y = 0;

            targetPosition += moveBackwardsAmount;
        }
        if (followY)
        {
            float distanceToMoveOnY = playerCamera.transform.position.y - transform.position.y <= 0 ? -1 * yDistance : 1 * yDistance;
            targetPosition.y = transform.position.y + distanceToMoveOnY; // Calculates the target location on the y axis
        }

        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, followSpeed * Time.deltaTime); // Smoothly goes to the target location
    }

    // Sets the cameras direction on the Y axis
    public void SetCameraRotationY(Vector3 targetEulers, bool instant = true)
    {
        Vector3 newEulers = new Vector3(playerCamera.eulerAngles.x, targetEulers.y, playerCamera.eulerAngles.z); // Calculates new eulers
        cameraDirection.eulerAngles = new Vector3(0, newEulers.y, 0); // Lets camera know which direction is forward
        playerCamera.eulerAngles = newEulers; // Sets the camera rotation to the new angle

        if (instant) // Should the camera be instantly reset?
        {
            ResetCameraLocation();
        }
    }

    //Sets the cameras direction in the x and z axis
    public void SetCameraRotationXZ(Vector3 targetEulers, bool instant = true)
    {
        Vector3 newEulers = new Vector3(targetEulers.x, playerCamera.eulerAngles.y, targetEulers.z); // Calculates new eulers
        playerCamera.eulerAngles = newEulers; // Sets the camera rotation to the new angle

        if (instant) // Should the camera be instantly reset?
        {
            ResetCameraLocation();
        }
    }

    public enum Role { TestRole, OtherTestRole} // Roles for spawn locations
}
