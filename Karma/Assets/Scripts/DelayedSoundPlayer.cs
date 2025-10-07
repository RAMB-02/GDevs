using UnityEngine;
using System.Collections;

public class DelayedSoundPlayer : MonoBehaviour
{
    [Tooltip("2초 뒤에 재생할 소리를 가진 AudioSource")]
    public AudioSource delayedAudioSource;
    public float delay = 2.0f;

    void Start()
    {
        // NecklessScript에 설정된 '신호'가 켜져 있는지 확인
        if (NecklessScript.playSoundOnNextSceneLoad)
        {
            // 신호를 받았으므로, 다시 사용을 위해 즉시 꺼줍니다.
            NecklessScript.playSoundOnNextSceneLoad = false;

            // 코루틴을 통해 소리를 지연 재생합니다.
            StartCoroutine(PlaySoundAfterDelay());
        }
    }

    private IEnumerator PlaySoundAfterDelay()
    {
        // 지정된 시간(2초)만큼 기다립니다.
        yield return new WaitForSeconds(delay);

        // AudioSource가 연결되어 있다면 소리를 재생합니다.
        if (delayedAudioSource != null)
        {
            delayedAudioSource.Play();
        }
    }
}