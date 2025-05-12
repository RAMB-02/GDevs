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
    public float jumpForce = 5f;     // 점프 초기 힘
    public float gravity = 9.81f;    // 중력 가속도

    private CharacterController controller;
    private Vector3 velocity;        // 플레이어의 실제 이동 속도 (x, y, z)

    // 지상에서 달리는 상태로 점프했는지, 걷는 상태로 점프했는지 구분하기 위해 사용
    private float currentAirSpeed;   // 공중에서 적용할 (수평) 이동 속도

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        // 1) 지상일 때: y축 속도를 약간 아래(-2f)로 세팅하여 바닥 진동 방지
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // 항상 이동 입력(좌우/앞뒤)은 받아서 moveDir을 구해두기
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;

        // 2) 지상일 때의 로직
        if (isGrounded)
        {
            // 달리기 여부 판단
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;

            // 수평 방향 속도 갱신
            velocity.x = moveDir.x * speed;
            velocity.z = moveDir.z * speed;

            // 점프 입력
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                // 점프 순간에, "이번 점프 동안 유지할 수평 속도"를 저장
                currentAirSpeed = speed;
            }
        }
        else
        {
            // 3) 공중일 때의 로직
            //   - 공중에서는 Shift를 눌러도 속도 바뀌지 않음
            //   - 대신 방향 전환은 어느 정도 가능하도록 함
            velocity.x = moveDir.x * currentAirSpeed;
            velocity.z = moveDir.z * currentAirSpeed;
        }

        // 4) 언제나 중력 적용
        velocity.y -= gravity * Time.deltaTime;

        // 5) 최종 이동 실행
        controller.Move(velocity * Time.deltaTime);
    }
}
