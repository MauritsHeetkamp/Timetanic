﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] SpawnManager playerHandler;
    [SerializeField] Transform globalCamera;
    [SerializeField] float playerBoundsOffset;

    [SerializeField] float cameraDistancePerUnit, defaultDistance;
    [SerializeField] float minCameraDistance, maxCameraDistance;
    [SerializeField] float maxPlayerDistance;

    [SerializeField] float zoomSmooth = 1;

    [SerializeField] GameObject splitscreen;
    [SerializeField] Transform splitscreenImageHolder;

    Vector3 targetLocation;
    bool isSplit = true;
    [SerializeField] float splitFadeDuration = 1;

    public bool handleTheCameras = true;
    FadeManager globalFader;
    // Start is called before the first frame update
    void Start()
    {
        globalFader = GameObject.FindGameObjectWithTag("GlobalFader").GetComponent<FadeManager>();
        targetLocation = globalCamera.position;
    }

    public void ResetCamera()
    {
        CameraMovement(true);
    }

    public void CheckSplit(bool instant = false)
    {
        if (GetGreatestDistance() < maxPlayerDistance)
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

    public void CheckSplit(float distance, bool instant = false)
    {
        if (distance < maxCameraDistance)
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

    public void InitializeSplitscreenCameras()
    {
        int playerAmount = playerHandler.localPlayers.Count;

        int requiredScreenAmount = 2;
        int powerOf = 2;

        if(playerAmount > 2)
        {
            for (int i = 2; true; i++)
            {
                requiredScreenAmount = i * i;

                if (requiredScreenAmount >= playerAmount)
                {
                    powerOf = i;
                    break;
                }
            }
        }

        float splitscreenSizeX = Screen.width / powerOf;
        float splitscreenSizeY = playerAmount > 2 ? Screen.height / powerOf : Screen.height;


        GridLayoutGroup layout = splitscreenImageHolder.GetComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(splitscreenSizeX, splitscreenSizeY);
        layout.constraintCount = playerAmount > 2 ? powerOf : 2;

        List<GameObject> newSplitscreens = new List<GameObject>();

        for(int i = 0; i < requiredScreenAmount; i++)
        {
            GameObject newSplitscreen = GameObject.Instantiate(splitscreen, splitscreenImageHolder);
            newSplitscreens.Add(newSplitscreen);

            if(i < playerAmount)
            {
                RenderTexture texture = new RenderTexture((int)splitscreenSizeX, (int)splitscreenSizeY, 0);
                playerHandler.localPlayers[i].playerCamera.GetComponent<Camera>().targetTexture = texture;
                playerHandler.localPlayers[i].attachedSplitscreen = newSplitscreen;
                newSplitscreen.GetComponent<RawImage>().color = Color.white;
                newSplitscreen.GetComponent<RawImage>().texture = texture;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraMovement();
    }

    void CameraMovement(bool instantMove = false)
    {
        if (globalCamera != null && playerHandler.localPlayers.Count > 0 && handleTheCameras)
        {
            Vector3 center = Vector3.zero;

            foreach (Player player in playerHandler.localPlayers)
            {
                center += player.transform.position;
            }

            center /= playerHandler.localPlayers.Count;

            targetLocation = center;

            float distance = GetGreatestDistance();
            float cameraDistance = distance * cameraDistancePerUnit;


            if (cameraDistance > maxCameraDistance)
            {
                cameraDistance = maxCameraDistance;
            }
            else
            {
                if (cameraDistance < minCameraDistance)
                {
                    cameraDistance = minCameraDistance;
                }
            }

            Vector3 targetDistance = -globalCamera.forward * (cameraDistance + defaultDistance);

            targetLocation = targetLocation + targetDistance;

            globalCamera.position = instantMove ? targetLocation : Vector3.Lerp(globalCamera.position, targetLocation, zoomSmooth);

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
    float GetGreatestDistance()
    {
        float biggestDistance = 0;


        foreach(Player player in playerHandler.localPlayers)
        {
            foreach (Player playerComparingWith in playerHandler.localPlayers)
            {
                float distance = Vector3.Distance(player.transform.position, playerComparingWith.transform.position);

                if(distance > biggestDistance)
                {
                    biggestDistance = distance;
                }
            }
        }
        return biggestDistance;
    }

    public void SetSplit(bool split, bool instant)
    {
        isSplit = split;
        if (!instant)
        {
            FadePanel fader = globalFader.FadeInOut(splitFadeDuration);
            if (split)
            {
                fader.onFadedIn += Split;
            }
            else
            {
                fader.onFadedIn += Unsplit;
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

    void Split()
    {
        splitscreenImageHolder.gameObject.SetActive(true);
        globalCamera.GetComponent<Camera>().enabled = false;
    }

    void Unsplit()
    {
        splitscreenImageHolder.gameObject.SetActive(false);
        globalCamera.GetComponent<Camera>().enabled = true;
    }
}
