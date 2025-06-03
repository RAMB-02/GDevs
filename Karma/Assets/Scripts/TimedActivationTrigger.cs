using UnityEngine;

public class TimedActivationTrigger : MonoBehaviour
{
    [Header("Ȱ��ȭ�� ������Ʈ")]
    public GameObject targetObject;

    [Header("Ȱ��ȭ ���� �ð� (��)")]
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

        // ������Ʈ�� ���� ������ ���� ������ ����
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        CancelInvoke(); // Ȥ�� �����ִ� Invoke ȣ�⵵ ���
    }
}
