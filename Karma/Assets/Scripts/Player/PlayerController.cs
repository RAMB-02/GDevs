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

    [Header("Stamina Settings")]
    public float maxStamina = 120f;           // 최대 스태미나
    public float staminaDrainRate = 20f;      // 초당 스태미나 소모량
    public float staminaRecoveryRate = 60f;   // 초당 스태미나 회복량
    public float exhaustionCooldown = 2.0f;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentAirSpeed;

    // 스태미나 관련 변수들
    private float currentStamina;
    private bool isRunning = false;   // 현재 달리고 있는지 여부
    private bool wantsToRun = false;
    private bool isExhausted = false; 
    private float exhaustionTimer = 0f; 

    // 스태미나 상태를 외부에서 확인할 수 있는 프로퍼티들
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float StaminaPercentage => currentStamina / maxStamina;
    public bool IsRunning => isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina; // 시작할 때 스태미나 최대치로 설정
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

        // 달리기 입력 확인 및 스태미나 체크
        wantsToRun = Input.GetKey(KeyCode.LeftShift) && moveDir.magnitude > 0.1f;
        bool canRun = currentStamina > 0f && !isExhausted;
        
        // 실제로 달릴 수 있는지 결정
        isRunning = wantsToRun && canRun && isGrounded;

        // 2) 땅 위일 때
        if (isGrounded)
        {
            //bool isRunning = Input.GetKey(KeyCode.LeftShift);
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

        HandleStamina();

        // 4) 중력 적용
        velocity.y -= gravity * Time.deltaTime;

        // 5) 최종 이동
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleStamina()
    {
        // 1. 고갈(Exhausted) 상태 타이머 관리
        if (isExhausted)
        {
            // 타이머 시간 감소
            exhaustionTimer -= Time.deltaTime;
            if (exhaustionTimer <= 0f)
            {
                isExhausted = false; // 고갈 상태 해제
            }
        }

        // 2. 스태미나 소모 조건
        
        bool canDrain = wantsToRun && !isExhausted && currentStamina > 0;

        if (canDrain)
        {
            // 스태미나 소모
            currentStamina -= staminaDrainRate * Time.deltaTime;

            // 방금 스태미나를 0까지 다 썼는지 체크
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true; // 고갈 상태 시작!
                exhaustionTimer = exhaustionCooldown; // 2초 타이머 설정
            }
        }
        else
        {
            // 3. 스태미나 회복 (소모 중이 아닐 때)
            
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina); 
        }
    }
    // 스태미나를 특정 값으로 설정하는 메서드 (필요시 사용)
    public void SetStamina(float stamina)
    {
        currentStamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    // 스태미나를 최대치로 회복하는 메서드
    public void RestoreStamina()
    {
        currentStamina = maxStamina;
    }
}