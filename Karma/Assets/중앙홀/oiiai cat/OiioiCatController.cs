using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class OiioiCatController : MonoBehaviour
{
    [Header("감지 및 추적 설정")]
    public float detectionRange = 4f;
    public float orbitDistance = 1.5f;

    [Header("속도 설정")]
    public float selfRotationSpeed = 500f;
    public float orbitSpeed = 200f;

    [Header("가속 배율 설정")]
    public float boostedSelfRotationMultiplier = 8f;
    public float boostedOrbitSpeedMultiplier = 4f;

    [Header("사운드 설정")]
    public AudioClip oiioiSound;

    private Transform playerTransform;
    private AudioSource audioSource;
    private Rigidbody rb;
    private Vector3 initialPosition;
    // initialPosition 변수는 더 이상 필요 없으므로 삭제해도 됩니다.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        initialPosition = rb.position;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
        else
        {
            Debug.LogError("오류: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            this.enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = oiioiSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        float currentSelfRotationSpeed = selfRotationSpeed;
        float currentOrbitSpeed = orbitSpeed;

        float distanceToPlayer = Vector3.Distance(rb.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // 감지 시: 속도 가속 및 공전
            currentSelfRotationSpeed *= boostedSelfRotationMultiplier;
            currentOrbitSpeed *= boostedOrbitSpeedMultiplier;

            OrbitPlayer(currentOrbitSpeed);

            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            // [핵심 변경점] 감지 범위 밖일 때:
            // ReturnToStartPosition() 함수 호출을 삭제했습니다.
            // 이제 그 자리에 멈춰서 자전만 합니다.
            if (audioSource.isPlaying) audioSource.Stop();
        }

        // 항상 자전은 실행됩니다. (감지 시에는 빠른 속도, 평소에는 보통 속도)
        ApplyRotation(currentSelfRotationSpeed);
    }

    void OrbitPlayer(float speed)
    {
        Quaternion orbitRotation = Quaternion.Euler(0, speed * Time.fixedDeltaTime, 0);
        Vector3 directionToCat = rb.position - playerTransform.position;
        Vector3 nextDirection = orbitRotation * directionToCat;
        Vector3 targetPosition = playerTransform.position + nextDirection.normalized * orbitDistance;
        rb.MovePosition(targetPosition);
    }

    // ReturnToStartPosition() 함수는 더 이상 사용되지 않으므로 삭제했습니다.

    void ApplyRotation(float speed)
    {
        Quaternion deltaRotation = Quaternion.Euler(0, speed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    public void ResetCat()
{
    // Rigidbody의 위치를 게임 시작 시 저장해둔 초기 위치로 순간이동시킵니다.
    if (rb != null)
    {
        rb.position = initialPosition;
    }
    else // 혹시 Rigidbody가 없는 경우를 대비한 코드
    {
        transform.position = initialPosition;
    }

    // 플레이어가 멀어지면 소리는 어차피 꺼지므로, 위치만 리셋하면 됩니다.
}
}