using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundClip; // ����� �Ҹ� Ŭ��
    private AudioSource audioSource; // AudioSource ������Ʈ

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource ������Ʈ�� ������ �߰�
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �Ҹ� ���
            audioSource.PlayOneShot(soundClip);
        }
    }
}
