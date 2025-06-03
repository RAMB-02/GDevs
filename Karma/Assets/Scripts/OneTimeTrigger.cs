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
            Debug.Log($"{gameObject.name} 사운드 트리거 발동!");

            if (sound != null)
            {
                sound.Play();
                StartCoroutine(DisableAfterSound());
            }
            else
            {
                Debug.LogWarning("AudioSource가 연결되어 있지 않습니다.");
                gameObject.SetActive(false); // 사운드 없으면 그냥 끔
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
