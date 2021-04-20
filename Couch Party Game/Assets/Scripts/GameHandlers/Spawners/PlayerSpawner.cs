﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] allCharacters; // All controllable character prefabs
    [SerializeField] RoleSpawnLocation[] allRoleSpawnLocations; // All spawn locations (role based)
    SpawnLocation[] allSpawnLocations;
    [SerializeField] SpawnLocation defaultSpawn; // Default spawn when no other location is available
    [SerializeField] bool onlyUniqueCharacters = true; // Can people play with the same character skin

    public List<Player> localPlayers = new List<Player>();
    [HideInInspector] public List<Player> globalPlayers = new List<Player>(); // all players included networked (for photon)
    [HideInInspector] public GameObject lastLocalPlayer; // Last instantiated local player

    [SerializeField] CameraHandler cameraHandler; // Instance that handles the cameras for everyone


    private void Awake()
    {
        List<SpawnLocation> allSpawns = new List<SpawnLocation>();

        foreach(RoleSpawnLocation roleLocation in allRoleSpawnLocations)
        {
            foreach(SpawnLocation spawnLocation in roleLocation.locations)
            {
                allSpawns.Add(spawnLocation);
            }
        }

        allSpawnLocations = allSpawns.ToArray();
    }

    // Gets the spawn data and spawns in the end
    public void GetSpawnData()
    {
        if(allCharacters.Length > 0 && defaultSpawn != null) // Requires at least one character and a default spawn
        {
            List<GameObject> availableCharacters = new List<GameObject>(allCharacters);
            List<RoleSpawnLocation> availableSpawnLocations = new List<RoleSpawnLocation>(allRoleSpawnLocations);

            for (int i = 0; i < PlayerManager.instance.connectedToLobbyPlayers.Count; i++) // Goes through all players
            {
                PlayerData thisPlayerData = PlayerManager.instance.connectedToLobbyPlayers[i]; // Gets playerdata from selected player

                if(thisPlayerData == null)
                {
                    continue;
                }

                GameObject selectedCharacter = availableCharacters[Random.Range(0, availableCharacters.Count)]; // Selects character

                if (onlyUniqueCharacters && availableCharacters.Count > 1) // Should the character be removed from the available characters
                {
                    availableCharacters.Remove(selectedCharacter);
                }

                Player.Role characterRole = selectedCharacter.GetComponent<Player>().role;

                foreach(RoleSpawnLocation roleLocation in availableSpawnLocations) // Goes through all spawn locations
                {
                    if(roleLocation.requiredRole == characterRole) // Checks if the player role matches the spawn locations required role
                    {
                        SpawnLocation selectedSpawn = defaultSpawn;

                        if (roleLocation.locations.Length > 0)
                        {
                            selectedSpawn = roleLocation.locations[Random.Range(0, roleLocation.locations.Length)]; // Selects random spawn location
                            if (roleLocation.locations.Length > 1) // Should the spawn location be removed from available spawn locations?
                            {
                                roleLocation.RemoveLocation(selectedSpawn);
                            }
                        }

                        ActualSpawn(selectedSpawn, selectedCharacter, i); // Actually spawns the player based on the received data
                    }
                }
            }
        }

        cameraHandler.InitializeSplitscreenCameras();
        cameraHandler.handleTheCameras = true; // Enables automatic camera handling
        cameraHandler.ResetCamera();
    }

    public void Respawn(Player target)
    {
        List<SpawnLocation> safeSpawns = new List<SpawnLocation>(allSpawnLocations);

        for(int i = safeSpawns.Count - 1; i >= 0; i--)
        {
            if (!safeSpawns[i].safe)
            {
                safeSpawns.RemoveAt(i);
            }
        }

        SpawnLocation selectedSpawn = safeSpawns[Random.Range(0, safeSpawns.Count)];

        //Relocation

        target.transform.position = selectedSpawn.location.position;
        if (selectedSpawn.wantedCameraDirection != null)
        {
            selectedSpawn.wantedCameraDirection.ChangeCameraNoFade(target.gameObject);
        }
        
        //target.ResetCameraLocation();
    }

    // Spawns the player instance
    void ActualSpawn(SpawnLocation location, GameObject character, int playerIndex)
    {
        PlayerData data = PlayerManager.instance.connectedToLobbyPlayers[playerIndex]; // Finds the playerdata from this player

        Player newCharacter = Instantiate(character, location.location.position, location.location.rotation).GetComponent<Player>(); // Spawns player and stores player script
        newCharacter.owner = data;
        newCharacter.playerSpawner = this;

        if(location.wantedCameraDirection != null)
        {
            location.wantedCameraDirection.ChangeCameraInstant(newCharacter.gameObject);
            newCharacter.ResetCameraLocation();
        }


        localPlayers.Add(newCharacter);
        globalPlayers.Add(newCharacter);
    }

    [System.Serializable]
    public class RoleSpawnLocation
    {
        public Player.Role requiredRole;
        public SpawnLocation[] locations;


        // Removes location from the available locations
        public void RemoveLocation(SpawnLocation target)
        {
            List<SpawnLocation> newLocations = new List<SpawnLocation>(locations);
            newLocations.Remove(target);
            locations = newLocations.ToArray();
        }

        // Removes location from the available locations
        public void RemoveLocation(int target)
        {
            List<SpawnLocation> newLocations = new List<SpawnLocation>(locations);
            newLocations.RemoveAt(target);
            locations = newLocations.ToArray();
        }
    }
}
