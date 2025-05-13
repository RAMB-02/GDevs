using System.Collections;
using UnityEngine;

public class DinoRoarTrigger : MonoBehaviour
{
    public Animator dinoAnimator;          // 공룡 애니메이터
    public AudioClip roarClip;             // 공룡 울음소리
    public AudioSource audioSource;        // 사운드 재생용 AudioSource
    public float delayBeforeRoar = 2f;     // 트리거 후 지연시간

    private bool hasActivated = false;     // 중복 방지용 플래그

    private void OnTriggerEnter(Collider other)
    {
        if (!hasActivated && other.CompareTag("Player"))
        {
            hasActivated = true;
            StartCoroutine(RoarAfterDelay());
        }
    }

    private IEnumerator RoarAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRoar);

        if (dinoAnimator != null)
        {
            dinoAnimator.SetTrigger("Roar");
        }

        if (audioSource != null && roarClip != null)
        {
            audioSource.clip = roarClip;
            audioSource.Play();
        }
    }
}