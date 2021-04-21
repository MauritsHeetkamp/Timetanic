using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLocation : MonoBehaviour
{
    public Location targetLocation;
    public Transform targetPosition;
    public Teleporter connectedTeleporter; // Teleporter that this is possibly teleporting to
}
