using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // ���콺 ����
    public Transform playerBody;          // �÷��̾� ��ü (�θ� ������Ʈ)

    private float xRotation = 0f;         // ī�޶��� ���� ȸ���� ����

    void Start()
    {
        // Ŀ���� ȭ�� �߾ӿ� ������Ű�� ����� �ɼ�
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1) ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2) �¿� ȸ��(Yaw) = �÷��̾� ��ü ȸ��
        //    "playerBody"�� ȸ�����Ѿ� ĳ���� ��ü�� �¿�� ������
        playerBody.Rotate(Vector3.up * mouseX);

        // 3) ���� ȸ��(Pitch) = ī�޶� ��ü�� ȸ��
        //    �Ʒ� �ڵ忡�� xRotation�� �����ؼ�, ī�޶��� localRotation�� �ٲ���
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
