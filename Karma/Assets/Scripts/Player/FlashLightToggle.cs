using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;                  // 손전등 Light 컴포넌트
    public KeyCode toggleKey = KeyCode.F;     // 손전등 토글 키

    public AudioSource audioSource;           // 소리 재생용 AudioSource
    public AudioClip toggleSound;             // 손전등 ON/OFF 소리

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            flashlight.enabled = !flashlight.enabled;

            // 소리 재생
            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
        }
    }
}
