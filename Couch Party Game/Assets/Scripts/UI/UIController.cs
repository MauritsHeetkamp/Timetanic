using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public int currentSelectedOption = -1; // Current option index that has been selected
    [SerializeField] UIOption[] allOptions;

    [SerializeField] float recenterThreshold = 0.8f; // Treshold before player can move the selection again
    bool horizontalRecentered = true; // Can the player move the selection horizontally?
    bool verticalRecentered = true; // Can the player move the selection vertically?

    float lastMoveAmount; // Last amount that has been moved in the horizontal axis

    [SerializeField] ScrollRect scroller;
    [SerializeField] float scrollSensitivity = 1;
    float scrollAmount;

    private void OnEnable()
    {
        Debug.Log("ADDED");
        PlayerManager.instance.onNewPlayerConnected += OnNewPlayerConnected;
        foreach(PlayerData data in PlayerManager.instance.connectedToPCPlayers)
        {
            data.onHorizontalAxis += MoveHorizontal;
            data.onVerticalAxis += MoveVertical;
            data.onSelect += Select;
            data.onScroll += Scroll;
        }
    }

    void Scroll(InputAction.CallbackContext context, PlayerData owner)
    {
        if(scroller != null)
        {
            if(context.started || context.canceled)
            {
                scrollAmount = context.ReadValue<float>();
            }
        }
    }

    private void OnDisable()
    {
        Debug.Log("REMOVED");
        PlayerManager.instance.onNewPlayerConnected -= OnNewPlayerConnected;
        foreach (PlayerData data in PlayerManager.instance.connectedToPCPlayers)
        {
            data.onHorizontalAxis -= MoveHorizontal;
            data.onVerticalAxis -= MoveVertical;
            data.onSelect -= Select;
            data.onScroll -= Scroll;
        }
    }

    public void OnNewPlayerConnected(PlayerData target)
    {
        target.onHorizontalAxis += MoveHorizontal;
        target.onVerticalAxis += MoveVertical;
        target.onSelect += Select;
        target.onScroll += Scroll;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(UIOption option in allOptions)
        {
            option.ownerController = this; // Initializes option
        }
        SetSelected(0); // Selects the first option
    }

    // Selects the current option
    public void Select(InputAction.CallbackContext context, PlayerData owner)
    {
        if (context.started)
        {
            allOptions[currentSelectedOption].Interact();
        }
    }

    // Moves the selection vertically by requested amount
    public void MoveVertical(InputAction.CallbackContext context, PlayerData owner)
    {
        Debug.Log("VER");
        float direction = context.ReadValue<float>(); // Checks the direction the button is pressed
        if (verticalRecentered) // Can the selection be moved vertically
        {
            if (context.performed && allOptions.Length > 0)
            {
                if (Mathf.Abs(direction) == 1) // Is the direction fully pressed
                {
                    verticalRecentered = false;
                    int targetOption = currentSelectedOption + -(int)direction; // Gets target option

                    if (targetOption >= allOptions.Length)
                    {
                        targetOption = 0; // Brings back target option to the start if it exceeded the limit
                    }
                    if (targetOption < 0)
                    {
                        targetOption = allOptions.Length - 1; // Brings the target option to the limit when it went below 0
                    }

                    SetSelected(targetOption); // Selects the new option
                }
            }
        }
        else
        {
            if(direction <= recenterThreshold && direction >= -recenterThreshold) // Is the button in between the required threshold
            {
                verticalRecentered = true;
            }
        }


    }

    // Moves the selection horizontal by requested amount
    public void MoveHorizontal(InputAction.CallbackContext context, PlayerData owner)
    {
        if(allOptions.Length > 0)
        {
            float direction = context.ReadValue<float>(); // Checks the direction the button is pressed
            lastMoveAmount = direction;

            if (horizontalRecentered) // Can the selection be moved horizontally
            {
                if (context.performed && Mathf.Abs(direction) == 1) // Is the direction fully pressed
                {
                    UIOption target = allOptions[currentSelectedOption]; // Gets target option

                    horizontalRecentered = false;
                    target.OnMovedHorizontal((int)direction); // Tells the target it moved horizontally
                }
            }
            else
            {
                if (direction <= recenterThreshold && direction >= -recenterThreshold) // Is the button in between the required threshold
                {
                    horizontalRecentered = true;
                }
            }
        }


    }

    // Sets the selected option
    public void SetSelected(int index)
    {
        if(currentSelectedOption != index) // Isnt this already the selected option?
        {
            if (index < allOptions.Length && index >= 0) // Is the option in range of the array
            {
                UIOption option;
                if (currentSelectedOption >= 0) // Was there an option already selected?
                {
                    option = allOptions[currentSelectedOption]; // Gets the current selected option
                    option.OnLeaveHover(); // Tells the current selected option that it is being deselected
                }

                currentSelectedOption = index; // Sets the new current selected option
                option = allOptions[currentSelectedOption];

                option.OnHover(); // Tells the new selected option that it has been selected
            }
        }
    }

    // Sets the selected option
    public void SetSelected(UIOption optionToSelect)
    {
        if(allOptions[currentSelectedOption] != optionToSelect) // Isnt this already the selected option?
        {
            for (int i = 0; i < allOptions.Length; i++)
            {
                UIOption selectedOption = allOptions[i];

                if (optionToSelect == selectedOption)
                {
                    UIOption option;
                    if (currentSelectedOption >= 0) // Was there an option already selected?
                    {
                        option = allOptions[currentSelectedOption]; // Gets the current selected option
                        option.OnLeaveHover(); // Tells the current selected option that it is being deselected
                    }

                    currentSelectedOption = i; // Sets the new current selected option
                    option = allOptions[currentSelectedOption];

                    option.OnHover(); // Tells the new selected option that it has been selected
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lastMoveAmount != 0)
        {
            UIOption option = allOptions[currentSelectedOption];
            option.OnMovedHorizontalStay(lastMoveAmount); // Tells the current option that it moved horizontally
        }

        if(scroller != null)
        {
            scroller.verticalNormalizedPosition += scrollAmount * Time.deltaTime * scrollSensitivity;
        }
    }
}
