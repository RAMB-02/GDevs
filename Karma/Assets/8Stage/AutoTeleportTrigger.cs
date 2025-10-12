using UnityEngine;
using UnityEngine.UI; // UI(Image)를 제어하기 위해 필수!
using TMPro;          // TextMeshPro를 제어하기 위해 필수!
using System.Collections;

public class AutoTeleportTrigger : MonoBehaviour
{
    [Header("핵심 연결 요소")]
    public GameObject playerObject;         // 플레이어 오브젝트
    public GameObject roomToDeactivate;     // 꺼질 방 (박물관)
    public GameObject roomToActivate;       // 켜질 방 (복도)
    public Transform spawnPoint;            // 복도의 스폰 위치

    [Header("시네마틱 연출")]
    public Camera playerCamera;             // 플레이어의 메인 카메라
    public float zoomFOV = 20f;             // 줌인 했을 때의 FOV 값 (작을수록 확대됨)
    public Transform lookAtTarget;          // 플레이어가 강제로 쳐다볼 대상
    public float preSequenceDelay = 1.0f;   // 연출 시작 전 대기 시간 (초)
    public float stareDuration = 2.5f;      // 보스와 문구를 쳐다볼 시간 (초)
    public AudioSource teleportSfx;         // 재생할 효과음
    public Image fadeImage;                 // 암전 효과를 위한 UI 이미지
    public float fadeDuration = 1.0f;       // 암전 해제 속도 (초)
    public float blackoutDuration = 1.0f;   // 암전이 지속되는 시간 (초)
    public GameObject quoteTextObject;      // 문구를 표시할 UI 텍스트 오브젝트

    [Header("손전등 제어")]
    public FlashlightToggle flashlightScript; // 플레이어의 FlashlightToggle 스크립트

    [Header("오브젝트 바꿔치기")]
    public GameObject objectToHide;         // 숨길 원래 오브젝트
    public GameObject bossObjectToShow;     // 나타날 보스 오브젝트

    [Header("플레이어 제어 스크립트 이름")]
    public string playerMovementScriptName = "PlayerController"; // 플레이어 움직임 스크립트 이름
    public string cameraLookScriptName = "FPSCamera";         // 카메라 시점 스크립트 이름

    private bool hasTriggered = false;      // 중복 실행 방지용 변수
    private float originalFOV;              // 원래 FOV 값을 저장할 변수

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(CinematicTeleport());
        }
    }

    private IEnumerator CinematicTeleport()
    {
        // --- 1. 모든 제어권 빼앗기 (움직임, 시점, 손전등) ---
        MonoBehaviour playerMovement = playerObject.GetComponent(playerMovementScriptName) as MonoBehaviour;
        MonoBehaviour cameraLook = playerObject.GetComponentInChildren<FPSCamera>();
        if (playerMovement != null) playerMovement.enabled = false;
        if (cameraLook != null) cameraLook.enabled = false;
        if (flashlightScript != null)
        {
            if (flashlightScript.flashlight.enabled) flashlightScript.flashlight.enabled = false;
            flashlightScript.enabled = false;
        }
        if (playerCamera != null) originalFOV = playerCamera.fieldOfView;

        // --- 2. 연출 시작 전 잠시 대기 ---
        yield return new WaitForSeconds(preSequenceDelay);

        // --- 3. 오브젝트 바꿔치기 실행 ---
        if (objectToHide != null) objectToHide.SetActive(false);
        if (bossObjectToShow != null) bossObjectToShow.SetActive(true);

        // --- 4. 특정 방향으로 시선 고정 + 카메라 줌 인 ---
        if (lookAtTarget != null && playerCamera != null)
        {
            Quaternion startPlayerRotation = playerObject.transform.rotation;
            Quaternion startCameraRotation = playerCamera.transform.localRotation;
            Quaternion targetFullRotation = Quaternion.LookRotation(lookAtTarget.position - playerCamera.transform.position);
            Quaternion targetPlayerRotation = Quaternion.Euler(0, targetFullRotation.eulerAngles.y, 0);
            Quaternion targetCameraRotation = Quaternion.Euler(targetFullRotation.eulerAngles.x, 0, 0);
            
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                playerObject.transform.rotation = Quaternion.Slerp(startPlayerRotation, targetPlayerRotation, timer);
                playerCamera.transform.localRotation = Quaternion.Slerp(startCameraRotation, targetCameraRotation, timer);
                playerCamera.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, timer);
                yield return null;
            }
        }
        
        // --- 5. 문구 표시, 효과음 재생 및 응시 ---
        if (quoteTextObject != null) quoteTextObject.SetActive(true);
        if (teleportSfx != null) teleportSfx.Play();
        
        yield return new WaitForSeconds(stareDuration);

        if (quoteTextObject != null) quoteTextObject.SetActive(false);

        // --- 6. 화면 암전 (즉시) ---
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 1f); 
        }

        // --- 7. 암전 상태로 대기 ---
        yield return new WaitForSeconds(blackoutDuration);

        // --- 8. 순간이동 실행 (암전된 상태에서) ---
        if (roomToActivate != null) roomToActivate.SetActive(true);
        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        playerObject.transform.position = spawnPoint.position;
        playerObject.transform.rotation = spawnPoint.rotation;
        yield return null;
        if (cc != null) cc.enabled = true;
        if (roomToDeactivate != null) roomToDeactivate.SetActive(false);

        // --- 9. 플레이어 제어권 돌려주기 (손전등 제외) + 카메라 줌 아웃 ---
        if (playerMovement != null) playerMovement.enabled = true;
        if (cameraLook != null) cameraLook.enabled = true;
        if (playerCamera != null) playerCamera.fieldOfView = originalFOV;
        
        // --- 10. 화면 암전 풀기 (부드럽게) ---
        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(0f, fadeDuration));
        }
        
        // --- 11. 트리거 비활성화 ---
        gameObject.SetActive(false);
    }

    // 화면을 부드럽게 밝히는 코루틴
    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}