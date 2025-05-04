using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // 마우스 감도
    public Transform playerBody;          // 플레이어 몸체 (부모 오브젝트)

    private float xRotation = 0f;         // 카메라의 상하 회전값 누적

    void Start()
    {
        // 커서를 화면 중앙에 고정시키고 숨기는 옵션
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1) 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2) 좌우 회전(Yaw) = 플레이어 본체 회전
        //    "playerBody"를 회전시켜야 캐릭터 자체가 좌우로 움직임
        playerBody.Rotate(Vector3.up * mouseX);

        // 3) 상하 회전(Pitch) = 카메라 자체만 회전
        //    아래 코드에서 xRotation에 누적해서, 카메라의 localRotation을 바꿔줌
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
