using UnityEngine;

public class MoonOrbitController : MonoBehaviour
{
    [Header("필수 설정")]
    public Transform player;

    [Header("공전 설정")]
    [Tooltip("플레이어가 이 거리 안에 '처음' 들어오면 공전이 시작될 수 있습니다.")]
    public float activationDistance = 3f;
    [Tooltip("플레이어로부터 유지할 공전 거리")]
    public float orbitDistance = 4f;
    [Tooltip("공전 속도 (초당 각도)")]
    public float orbitSpeed = 45f;
    [Tooltip("공전 거리 보정 속도")]
    public float distanceCorrectionSpeed = 2f;
    [Tooltip("플레이어 기준 로컬 공전 축")]
    public Vector3 localOrbitAxis = new Vector3(0.3f, 1f, 0);

    // 상태 변수들
    private bool isAnomalyCurrentlyActive = false; // AnomalyManager에 의해 제어됨 (OnEnable/OnDisable)
    private bool hasOrbitBeenActivated = false; // 공전이 '한 번이라도' 시작되었는지 여부 (★새로운 변수 또는 역할 강화)
    private bool isOrbiting = false;            // 현재 실제로 공전 중인지 여부

    private Vector3 normalizedLocalOrbitAxis;

    [ContextMenu("Normalize Local Orbit Axis")]
    void NormalizeLocalOrbitAxis()
    {
        if (localOrbitAxis.magnitude > 0) normalizedLocalOrbitAxis = localOrbitAxis.normalized;
        else normalizedLocalOrbitAxis = Vector3.up;
        localOrbitAxis = normalizedLocalOrbitAxis;
    }

    void OnEnable()
    {
        isAnomalyCurrentlyActive = true;
        // ★★★ 중요: 오브젝트가 다시 활성화될 때마다 공전 시작 여부를 리셋해야 합니다. ★★★
        hasOrbitBeenActivated = false;
        isOrbiting = false;
        // ---
        if (localOrbitAxis.magnitude == 0) localOrbitAxis = Vector3.up;
        normalizedLocalOrbitAxis = localOrbitAxis.normalized;
        Debug.Log("MoonOrbitController: 활성화됨 (Anomaly Enabled), 공전 상태 초기화됨.");
    }

    void OnDisable()
    {
        isAnomalyCurrentlyActive = false;
        isOrbiting = false;
        hasOrbitBeenActivated = false; // 비활성화 시 리셋
        Debug.Log("MoonOrbitController: 비활성화됨 (Anomaly Disabled)");
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("MoonOrbitController: 'Player' Transform이 할당되지 않았습니다!", this);
            gameObject.SetActive(false); return;
        }
        isAnomalyCurrentlyActive = gameObject.activeInHierarchy;
        // 시작 시에도 상태 초기화
        hasOrbitBeenActivated = false;
        isOrbiting = false;
        if (localOrbitAxis.magnitude == 0) localOrbitAxis = Vector3.up;
        normalizedLocalOrbitAxis = localOrbitAxis.normalized;
    }

    void Update()
    {
        // 이상현상이 비활성화 상태거나 플레이어가 없으면 실행 안 함
        if (!isAnomalyCurrentlyActive || player == null) return;

        // ---! 상태 결정 로직 수정 !---
        // 1. 아직 공전이 시작된 적 없다면(hasOrbitBeenActivated == false), 플레이어 거리 체크
        if (!hasOrbitBeenActivated)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            // 플레이어가 활성화 거리 안으로 들어왔다면 공전 시작!
            if (distanceToPlayer <= activationDistance)
            {
                Debug.Log("플레이어 최초 감지! 달 공전 시작.");
                hasOrbitBeenActivated = true; // "이제 공전 시작됨" 상태로 변경
                isOrbiting = true;            // 실제 공전 실행 플래그 켜기
            }
        }

        // 2. 일단 공전이 시작되었다면 (hasOrbitBeenActivated == true),
        //    isOrbiting 상태는 계속 true로 유지됩니다.
        //    플레이어가 멀어져도 isOrbiting = false 로 바꾸는 코드가 없습니다.
        //    오직 OnDisable 될 때만 isOrbiting = false 가 됩니다.

        // --- 공전 실행 ---
        // isOrbiting 플래그가 true일 때만 공전 로직 실행
        if (isOrbiting)
        {
            PerformOrbit();
        }
    }

    // 실제 공전 로직 (변경 없음)
    void PerformOrbit()
    {
        Vector3 worldOrbitAxis = player.TransformDirection(normalizedLocalOrbitAxis);
        transform.RotateAround(player.position, worldOrbitAxis, orbitSpeed * Time.deltaTime);
        Vector3 directionFromPlayer = (transform.position - player.position).normalized;
        Vector3 desiredPosition = player.position + directionFromPlayer * orbitDistance;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * distanceCorrectionSpeed);
        // transform.LookAt(player); // Optional
    }
}
