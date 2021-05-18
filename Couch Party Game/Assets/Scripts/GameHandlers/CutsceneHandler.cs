﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneHandler : MonoBehaviour
{
    [SerializeField] GameObject cutscenePrefab;
    [SerializeField] Transform cutsceneParent;

    public Cutscene StartNewCutscene(VideoClip video)
    {
        GameObject cutsceneObject = Instantiate(cutscenePrefab, cutsceneParent);
        Cutscene cutsceneComponent = cutsceneObject.GetComponent<Cutscene>();
        cutsceneComponent.PlayCutscene(video);

        return cutsceneComponent;
    }
}
