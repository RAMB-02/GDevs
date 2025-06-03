using UnityEngine;
using System.Collections;

public class OneTimeTrigger : MonoBehaviour
{
    public AudioSource sound;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log($"{gameObject.name} ���� Ʈ���� �ߵ�!");

            if (sound != null)
            {
                sound.Play();
                StartCoroutine(DisableAfterSound());
            }
            else
            {
                Debug.LogWarning("AudioSource�� ����Ǿ� ���� �ʽ��ϴ�.");
                gameObject.SetActive(false); // ���� ������ �׳� ��
            }
        }
    }

    IEnumerator DisableAfterSound()
    {
        yield return new WaitForSeconds(sound.clip.length);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        hasTriggered = false;
    }
}
