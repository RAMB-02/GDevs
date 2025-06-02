using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;

    [Header("Jump & Gravity")]
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentAirSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ⛔ CharacterController가 비활성화 상태면 이동 코드 실행 X
        if (!controller.enabled) return;

        bool isGrounded = controller.isGrounded;

        // 1) 땅에 닿아있고 y속도가 아래로 가면 가볍게 붙잡기
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // 이동 입력 받기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;

        // 2) 땅 위일 때
        if (isGrounded)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;

            velocity.x = moveDir.x * speed;
            velocity.z = moveDir.z * speed;

            // 점프 입력
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                currentAirSpeed = speed;
            }
        }
        else
        {
            // 3) 공중일 때: 이전 속도 유지
            velocity.x = moveDir.x * currentAirSpeed;
            velocity.z = moveDir.z * currentAirSpeed;
        }

        // 4) 중력 적용
        velocity.y -= gravity * Time.deltaTime;

        // 5) 최종 이동
        controller.Move(velocity * Time.deltaTime);
    }
}