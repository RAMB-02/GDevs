using UnityEngine;

public class ShowOnTrigger : MonoBehaviour
{
    public GameObject targetObject;     // ���� ������Ʈ
    public AudioClip soundEffect;       // ����� �Ҹ�
    public AudioSource audioSource;     // �Ҹ��� ����� ����� �ҽ�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������Ʈ ���̱�
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            // �Ҹ� ���
            if (audioSource != null && soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    }
}
