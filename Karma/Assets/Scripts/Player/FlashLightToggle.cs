using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;                  // ������ Light ������Ʈ
    public KeyCode toggleKey = KeyCode.F;     // ������ ��� Ű

    public AudioSource audioSource;           // �Ҹ� ����� AudioSource
    public AudioClip toggleSound;             // ������ ON/OFF �Ҹ�

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            flashlight.enabled = !flashlight.enabled;

            // �Ҹ� ���
            if (audioSource != null && toggleSound != null)
            {
                audioSource.PlayOneShot(toggleSound);
            }
        }
    }
}
