using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    public AudioSource footstepAudio;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    CharacterController controller;
    float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (IsWalking())
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval; // 정지 시 초기화
        }
    }

    bool IsWalking()
    {
        return controller.isGrounded && controller.velocity.magnitude > 0.1f;
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            footstepAudio.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudio.Play();
        }
    }
}
