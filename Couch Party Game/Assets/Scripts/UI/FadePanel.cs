using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public UnityAction onFadedIn, onFadedOut;
    public UnityAction<GameObject> onFadedInSpecificPlayer, onFadedOutSpecificPlayer;

    public Image fadePanel; // Image that fades away
}
