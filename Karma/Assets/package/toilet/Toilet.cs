using UnityEngine;

public class Toilet : MonoBehaviour
{
    public AudioSource fartsound;
    public AudioSource flushsound;

    public void PlaySound()
    {
        if (fartsound != null && !fartsound.isPlaying)
        {
            fartsound.Play();

        }
    }
    public void PlaySound2()
    {
        if (flushsound != null && !flushsound.isPlaying)
        {
            flushsound.Play();

        }
    }
}
