using UnityEngine;

public class EyeController2D : MonoBehaviour
{
    [Header("연결할 오브젝트")]
    public Transform player;
    public Transform pupil;

    [Header("설정값")]
    public float moveRadius; // 눈동자가 '초기 위치'에서 얼마나 더 움직일지 결정

    private bool isTracking = false;
    private Vector3 initialPupilPosition; // 눈동자의 '초기 위치'를 저장할 변수

    void Start()
    {
        if (pupil != null)
        {
            // 게임 시작 시, 씬 뷰에 설정된 눈동자의 위치를 저장
            initialPupilPosition = pupil.localPosition;
        }
    }

    void Update()
    {
        if (!isTracking || player == null || pupil == null) return;

        // 1. 눈의 중심에서 플레이어를 향하는 순수한 방향을 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 localDirection = transform.InverseTransformDirection(directionToPlayer);
        localDirection.z = 0;

        // 2. '초기 위치'에서 얼마나 움직일지에 대한 '이동량'을 계산
        Vector3 trackingOffset = localDirection * moveRadius;

        // 3. 최종 위치 = 저장해둔 '초기 위치' + 계산된 '이동량'
        pupil.localPosition = initialPupilPosition + trackingOffset;
    }

    public void StartTracking()
    {
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
        if (pupil != null)
        {
            // 추적 중지 시, 다시 원래의 '초기 위치'로 복귀
            pupil.localPosition = initialPupilPosition;
        }
    }
}