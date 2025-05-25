using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 80f; // ���콺 ����
    public Transform playerBody;          // �÷��̾� ��ü (�θ� ������Ʈ)

    private float xRotation = 0f;         // ī�޶��� ���� ȸ���� ����

    public float lookSpeed = 2.0f;

    void Start()
    {
        // Ŀ���� ȭ�� �߾ӿ� ������Ű�� ����� �ɼ�
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (PauseMenu.isPaused)
        {
            return;
        }

        // 1) ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed * SettingsPanelController.mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed * SettingsPanelController.mouseSensitivity;

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
