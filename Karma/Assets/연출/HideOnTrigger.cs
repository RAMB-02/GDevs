using UnityEngine;

public class HideOnTrigger : MonoBehaviour
{
    public GameObject targetObject;     // ���� ������Ʈ
    public AudioClip soundEffect;       // ����� �Ҹ�
    public AudioSource audioSource;     // �Ҹ��� ����� ����� �ҽ�

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            // ������Ʈ �����
            if (targetObject != null)
            {
                targetObject.SetActive(false);
            }

            // �Ҹ� ���
            if (audioSource != null && soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false; // Ʈ���� ���� �ʱ�ȭ

        if (targetObject != null)
            targetObject.SetActive(true);  // �� �ʱ�ȭ �� ������Ʈ �ٽ� Ȱ��ȭ
        else
            gameObject.SetActive(true);    // targetObject ������ �ڽ��� Ȱ��ȭ
    }
}
