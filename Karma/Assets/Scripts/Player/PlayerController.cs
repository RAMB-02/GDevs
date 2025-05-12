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
    public float jumpForce = 5f;     // ���� �ʱ� ��
    public float gravity = 9.81f;    // �߷� ���ӵ�

    private CharacterController controller;
    private Vector3 velocity;        // �÷��̾��� ���� �̵� �ӵ� (x, y, z)

    // ���󿡼� �޸��� ���·� �����ߴ���, �ȴ� ���·� �����ߴ��� �����ϱ� ���� ���
    private float currentAirSpeed;   // ���߿��� ������ (����) �̵� �ӵ�

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        // 1) ������ ��: y�� �ӵ��� �ణ �Ʒ�(-2f)�� �����Ͽ� �ٴ� ���� ����
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // �׻� �̵� �Է�(�¿�/�յ�)�� �޾Ƽ� moveDir�� ���صα�
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;

        // 2) ������ ���� ����
        if (isGrounded)
        {
            // �޸��� ���� �Ǵ�
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;

            // ���� ���� �ӵ� ����
            velocity.x = moveDir.x * speed;
            velocity.z = moveDir.z * speed;

            // ���� �Է�
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                // ���� ������, "�̹� ���� ���� ������ ���� �ӵ�"�� ����
                currentAirSpeed = speed;
            }
        }
        else
        {
            // 3) ������ ���� ����
            //   - ���߿����� Shift�� ������ �ӵ� �ٲ��� ����
            //   - ��� ���� ��ȯ�� ��� ���� �����ϵ��� ��
            velocity.x = moveDir.x * currentAirSpeed;
            velocity.z = moveDir.z * currentAirSpeed;
        }

        // 4) ������ �߷� ����
        velocity.y -= gravity * Time.deltaTime;

        // 5) ���� �̵� ����
        controller.Move(velocity * Time.deltaTime);
    }
}
