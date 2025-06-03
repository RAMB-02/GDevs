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
    }

    public void ResetTrigger()
    {
        isTriggered = false;

        // 오브젝트가 켜진 상태일 수도 있으니 꺼줌
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        CancelInvoke(); // 혹시 남아있던 Invoke 호출도 취소
    }
}
