using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ScreenFlashTrigger : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public float interval = 0.4f;
    public bool repeatForever = true;

    private bool isTriggered = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false; // 반복 재생하지 않도록 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && fadeScreen != null && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(RepeatFlash());
        }
    }

    private IEnumerator RepeatFlash()
    {
        while (repeatForever)
        {
            //  소리 재생
            if (audioSource.clip != null)
                audioSource.Play();

            //  화면 깜빡임
            yield return StartCoroutine(fadeScreen.Flash(null));

            yield return new WaitForSeconds(interval);
        }
    }
}