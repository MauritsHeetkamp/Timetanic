using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "Create CharacterData")]
public class PlayerCharacterData : ScriptableObject
{
    public GameObject lobbyInstance;
    public GameObject gameInstance;
    public Player.Role role;
}
