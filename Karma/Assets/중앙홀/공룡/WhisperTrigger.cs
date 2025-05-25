using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WhisperTrigger : MonoBehaviour
{
    private AudioSource whisperAudio;

    private void Start()
    {
        whisperAudio = GetComponent<AudioSource>();
        whisperAudio.playOnAwake = false;
        whisperAudio.loop = true;
        whisperAudio.spatialBlend = 1.0f; // 3D 사운드
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!whisperAudio.isPlaying)
            {
                whisperAudio.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (whisperAudio.isPlaying)
            {
                whisperAudio.Stop();
            }
        }
    }
}