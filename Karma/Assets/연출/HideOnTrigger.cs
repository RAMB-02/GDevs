using UnityEngine;

public class HideOnTrigger : MonoBehaviour
{
    public GameObject targetObject;     // ���� ������Ʈ
    public AudioClip soundEffect;       // ����� �Ҹ�
    public AudioSource audioSource;     // �Ҹ��� ����� ����� �ҽ�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
}
