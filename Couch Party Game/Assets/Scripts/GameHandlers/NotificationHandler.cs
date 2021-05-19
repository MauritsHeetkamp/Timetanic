using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationHandler : MonoBehaviour
{
    [SerializeField] AnimationClip startClip, endClip;
    [SerializeField] Animator animator;
    [SerializeField] string animatorTrigger = "Notification";
    [SerializeField] TextMeshProUGUI notificationText;

    Coroutine currentNotificationRoutine;

    public void ActivateNotification(NotificationData notification)
    {
        if(currentNotificationRoutine == null && notification.notificationDuration > 0 && !string.IsNullOrEmpty(notification.notificationText))
        {
            currentNotificationRoutine = StartCoroutine(NotificationRoutine(notification));
        }
    }

    IEnumerator NotificationRoutine(NotificationData data)
    {
        notificationText.text = data.notificationText;
        animator.SetTrigger(animatorTrigger);
        yield return new WaitForSeconds(startClip.length + data.notificationDuration + endClip.length);
        animator.SetTrigger(animatorTrigger);
        currentNotificationRoutine = null;
    }

    [System.Serializable]
    public struct NotificationData
    {
        public string notificationText;
        public float notificationDuration;

        public NotificationData(string _notificationText, float _notificationDuration)
        {
            notificationText = _notificationText;
            notificationDuration = _notificationDuration;
        }
    }
}
