using UnityEngine;
using UnityEngine.Audio;

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundClip;
    public AudioMixerGroup sfxOutputGroup;
    private AudioSource audioSource;

    private bool hasTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxOutputGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxOutputGroup;
        }
        else
        {
            Debug.LogWarning("SFX Output Group is not assigned in SoundTrigger on " + gameObject.name + ". Sound will not be controlled by this mixer group.");
        }

        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (soundClip != null)
            {
                audioSource.PlayOneShot(soundClip);
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;

        // Stop any sound still playing if necessary
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
