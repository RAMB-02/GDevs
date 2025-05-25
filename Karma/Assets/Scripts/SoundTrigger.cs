using UnityEngine;
using UnityEngine.Audio; // AudioMixerGroup 사용을 위해 추가

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundClip;
    public AudioMixerGroup sfxOutputGroup; // 인스펙터에서 연결할 AudioMixerGroup (예: MainMixer의 SFX 그룹)
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioSource의 Output을 지정된 Mixer Group으로 설정
        if (sfxOutputGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxOutputGroup;
        }
        else
        {
            // sfxOutputGroup이 할당되지 않았다면 경고를 남기고,
            // 필요하다면 기본 Master 그룹을 찾아 할당할 수도 있습니다. (더 복잡해질 수 있음)
            Debug.LogWarning("SFX Output Group is not assigned in SoundTrigger on " + gameObject.name + ". Sound will not be controlled by this mixer group.");
        }

        // 기타 AudioSource 초기 설정 (예: PlayOnAwake 해제 등)
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (soundClip != null)
            {
                // PlayOneShot은 해당 AudioSource의 기본 설정을 따르므로 outputAudioMixerGroup이 적용됩니다.
                audioSource.PlayOneShot(soundClip);
            }
        }
    }
}