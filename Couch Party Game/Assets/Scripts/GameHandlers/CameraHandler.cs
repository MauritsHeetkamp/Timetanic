using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CameraHandler : MonoBehaviour
{
    int forceSplit;

    [SerializeField] SpawnManager playerHandler; // The spawner keeps track of players
    public Transform globalCamera; // The global camera for all-in-one screen

    [SerializeField] float cameraDistancePerUnit, defaultDistance; // Camera distance to zoom per unit of distance between players, distance to add standard
    [SerializeField] float minCameraDistance, maxCameraDistance; // Minimal and maximal camera distance
    [SerializeField] float maxPlayerDistance; // Maximal distance between players before split screen is activated

    [SerializeField] float zoomSmooth = 1; // Smoothening speed of camera

    [SerializeField] GameObject splitscreen; // Split screen prefab
    [SerializeField] Transform splitscreenImageHolder; // Transform that holds all the splitscreens

    Vector3 targetLocation;
    public bool isSplit;
    [SerializeField] float splitFadeDuration = 1; // Duration of the fade when swapping between all-in-one and split screen (fade in and out combined)
    public UnityAction<bool> onSplitStateChanged;

    public bool handleTheCameras = true; // Should the cameras be handled automatically?
    bool singlePlayer;
    FadeManager globalFader; // The fade handler


    // Start is called before the first frame update
    void Start()
    {
        globalFader = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<FadeManager>();

        targetLocation = globalCamera.position;
    }

    public void ForceSplit(bool split)
    {
        if (split)
        {
            forceSplit++;
        }
        else
        {
            forceSplit--;
        }

        if (!singlePlayer)
        {
            CheckSplit();
        }
    }

    // Resets the camera location
    public void ResetCamera()
    {
        CameraMovement(true);
    }

    // Checks if the screen should be split
    public void CheckSplit(bool instant = false)
    {
        if(forceSplit <= 0)
        {
            if (GetGreatestDistance() < maxPlayerDistance) // Checks if everyone is in the maximum distance from eachother
            {
                if (isSplit)
                {
                    SetSplit(false, instant);
                }
            }
            else
            {
                if (!isSplit)
                {
                    SetSplit(true, instant);
                }
            }
        }
        else
        {
            if (!isSplit)
            {
                SetSplit(true, instant);
            }
        }
    }

    // Checks if the screen should be split
    public void CheckSplit(float distance, bool instant = false)
    {
        if (distance < maxCameraDistance) // Checks if everyone is in the maximum distance from eachother
        {
            if (isSplit)
            {
                SetSplit(false, instant);
            }
        }
        else
        {
            if (!isSplit)
            {
                SetSplit(true, instant);
            }
        }
    }

    // Creates the split screens and assigns them
    public void InitializeSplitscreenCameras()
    {
        int playerAmount = playerHandler.localPlayers.Count;

        int requiredScreenAmount = 1;
        int powerOf = 1;

        if (playerAmount == 2)
        {
            requiredScreenAmount = playerAmount;
            powerOf = playerAmount;
        }


        if(playerAmount > 2) // Checks if there should be more then 2 screens
        {
            for (int i = 2; true; i++)
            {
                requiredScreenAmount = i * i; // Screens should be the same size

                if (requiredScreenAmount >= playerAmount) // Enough screens to match the player amount
                {
                    powerOf = i;
                    break;
                }
            }
        }

        float splitscreenSizeX = Screen.width / powerOf; // Calculates the x size of one splitscreen
        float splitscreenSizeY = playerAmount > 2 ? Screen.height / powerOf : Screen.height; // Calculates the y size of one splitscreen


        GridLayoutGroup layout = splitscreenImageHolder.GetComponent<GridLayoutGroup>(); // Gets the grid layout element
        layout.cellSize = new Vector2(splitscreenSizeX, splitscreenSizeY); // Sets the size of the splitscreens
        layout.constraintCount = playerAmount > 2 ? powerOf : 2; // Sets the constraint amount

        List<GameObject> newSplitscreens = new List<GameObject>();

        for(int i = 0; i < requiredScreenAmount; i++)
        {
            GameObject newSplitscreen = Instantiate(splitscreen, splitscreenImageHolder); // Creates new splitscreen
            newSplitscreens.Add(newSplitscreen);

            if(i < playerAmount) // Checks if this splitscreen should be assigned to a player
            {
                RenderTexture texture = new RenderTexture((int)splitscreenSizeX, (int)splitscreenSizeY, 0); // Creates new rendertexture with appropriate size
                playerHandler.localPlayers[i].actualCameraTransform.GetComponent<Camera>().targetTexture = texture; // Assigns the players local camera to the rendertexture
                playerHandler.localPlayers[i].attachedSplitscreen = newSplitscreen.GetComponent<Splitscreen>(); // Lets the player know what splitscreen it is connected to
                newSplitscreen.GetComponent<Splitscreen>().splitscreenRenderImage.color = Color.white; // Makes sure the splitscreen isn't black
                newSplitscreen.GetComponent<Splitscreen>().splitscreenRenderImage.texture = texture; // Assigns the rendertexture to the splitscreen
                newSplitscreen.GetComponent<Splitscreen>().owner = playerHandler.localPlayers[i];
            }
        }


        if(playerAmount == 1)
        {
            singlePlayer = true;
            handleTheCameras = false;
        }
        SetSplit(true, true);
    }

    // FixedUpdate is called once per Time.deltaTime
    void FixedUpdate()
    {
        CameraMovement();
    }

    // Handles the camera movement
    void CameraMovement(bool instantMove = false)
    {
        if (globalCamera != null && playerHandler.localPlayers.Count > 1 && handleTheCameras) // Checks if there is a camera, enough players and if it should be handled
        {
            Vector3 center = Vector3.zero;

            foreach (Player player in playerHandler.localPlayers)
            {
                center += player.transform.position;
            }

            center /= playerHandler.localPlayers.Count; // Calculates the center

            targetLocation = center;

            float distance = GetGreatestDistance();
            float cameraDistance = distance * cameraDistancePerUnit; // Calculate the cameras wanted distance
            cameraDistance += defaultDistance;

            if (cameraDistance > maxCameraDistance) // Is the distance bigger then allowed?
            {
                cameraDistance = maxCameraDistance;
            }
            else
            {
                if (cameraDistance < minCameraDistance) // Is the distance smaller then allowed?
                {
                    cameraDistance = minCameraDistance;
                }
            }

            Vector3 targetDistance = -globalCamera.forward * cameraDistance; // Calculates distance vector

            targetLocation = targetLocation + targetDistance; // Combines center vector with distance vector for target location

            globalCamera.position = instantMove ? targetLocation : Vector3.Lerp(globalCamera.position, targetLocation, zoomSmooth); // Moves instant or smoothly to target

            if (instantMove)
            {
                CheckSplit(true);
            }
            else
            {
                CheckSplit(false);
            }

        }
    }

    // Gets greatest distance between players
    float GetGreatestDistance()
    {
        float biggestDistance = 0;


        foreach(Player player in playerHandler.localPlayers)
        {
            foreach (Player playerComparingWith in playerHandler.localPlayers)
            {
                float distance = Vector3.Distance(player.transform.position, playerComparingWith.transform.position); // Calculates distance between the two players

                if(distance > biggestDistance) // Is the distance bigger?
                {
                    biggestDistance = distance;
                }
            }
        }
        return biggestDistance;
    }

    // Sets split screen
    public void SetSplit(bool split, bool instant)
    {
        isSplit = split;
        if (!instant)
        {
            FadePanel fader = globalFader.FadeInOut(splitFadeDuration); // Fades screen to split
            if (split)
            {
                fader.onFadedIn += Split; // Splits when screen is faded in
            }
            else
            {
                fader.onFadedIn += Unsplit; // Unsplits when screen is faded in
            }
        }
        else
        {
            if (split)
            {
                Split();
            }
            else
            {
                Unsplit();
            }
        }
    }

    // Splits screen
    void Split()
    {
        if(onSplitStateChanged != null)
        {
            onSplitStateChanged.Invoke(true);
        }
        splitscreenImageHolder.gameObject.SetActive(true); // Enables split screens
        foreach(Player player in playerHandler.localPlayers)
        {
            player.actualCameraTransform.GetComponent<Camera>().enabled = true; // Enables all players their cameras
        }
        globalCamera.GetComponent<Camera>().enabled = false; // Disables global camera
    }

    // Recalculates the global cameras rotation
    public void RecalculateRotation()
    {
        List<GenericCounter<Vector3>> cameraEulers = new List<GenericCounter<Vector3>>(); // Creates counter

        foreach (Player player in playerHandler.localPlayers) // Goes through every spawned player
        {
            bool found = false;
            foreach (GenericCounter<Vector3> counter in cameraEulers)
            {
                if (counter.data == player.playerCameraHolder.eulerAngles)
                {
                    counter.Change(1); // increments counter with 1
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                cameraEulers.Add(new GenericCounter<Vector3>(player.playerCameraHolder.eulerAngles)); // Adds new counterdata
            }

        }

        Vector3 selectedEulers = globalCamera.eulerAngles;

        if (cameraEulers.Count > 0)
        {
            int highestAmount = -1;

            foreach (GenericCounter<Vector3> counter in cameraEulers) // selects counter with highest amount
            {
                if (counter.count > highestAmount)
                {
                    highestAmount = counter.count;
                    selectedEulers = counter.data;
                }
            }
        }

        Vector3 newEulers = new Vector3(globalCamera.eulerAngles.x, selectedEulers.y, globalCamera.eulerAngles.z); // New camera eulers based on amounts

        globalCamera.eulerAngles = newEulers;
    }

    void Unsplit()
    {
        if (onSplitStateChanged != null)
        {
            onSplitStateChanged.Invoke(false);
        }

        splitscreenImageHolder.gameObject.SetActive(false); // Disables split screens

        List<GenericCounter<Vector3>> cameraEulers = new List<GenericCounter<Vector3>>();

        foreach (Player player in playerHandler.localPlayers)
        {
            player.actualCameraTransform.GetComponent<Camera>().enabled = false; // Disables all players their cameras
        }

        globalCamera.GetComponent<Camera>().enabled = true; // Enables global camera
    }

    struct GenericCounter<T>
    {
        public int count;
        public T data;

        public GenericCounter(T _data, int _count = 1)
        {
            count = _count;
            data = _data;
        }

        public void Change(int amount)
        {
            count += amount;
        }
    }
}
