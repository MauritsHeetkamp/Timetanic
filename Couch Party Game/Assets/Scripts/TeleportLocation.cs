using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLocation : MonoBehaviour
{
    public Transform targetPosition;
    public Teleporter connectedTeleporter; // Teleporter that this is possibly teleporting to
}
