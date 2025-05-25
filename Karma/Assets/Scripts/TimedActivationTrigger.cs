using UnityEngine;

public class TimedActivationTrigger : MonoBehaviour
{
    [Header("활성화할 오브젝트")]
    public GameObject targetObject;

    [Header("활성화 지속 시간 (초)")]
    public float activeDuration = 3f;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            ActivateObjectTemporarily();
        }
    }

    private void ActivateObjectTemporarily()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Invoke(nameof(DeactivateObject), activeDuration);
        }
    }

    private void DeactivateObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        isTriggered = false; // 다시 작동할 수 있도록 초기화 (원하면 제거 가능)
    }
}
