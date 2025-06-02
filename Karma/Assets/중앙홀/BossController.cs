using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("이동 관련")]
    public Transform player;
    public float moveSpeed = 3f;
    public float detectionDistance = 2f;

    [Header("사망 처리")]
    public PlayerDeath playerDeath;

    [Header("초기 비활성화 오브젝트")]
    public AudioSource tensionMusicSource;
    public Light[] controlledLights;

    private Animator animator;
    private bool canMove = false;
    private bool isDark = false;
    private bool isKilling = false;
    private bool hasInitializedLastPos = false; // ← 추가됨
    private Vector3 lastPlayerPosition;
    private float moveCheckTimer = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (tensionMusicSource != null)
            tensionMusicSource.Stop();

        if (controlledLights != null)
        {
            foreach (var light in controlledLights)
                if (light != null) light.enabled = false;
        }
    }

    void Update()
    {
        Debug.Log($"[보스] canMove={canMove}, isDark={isDark}, player={(player != null)}");

        if (isKilling || player == null)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        // 어둠 상태에서 움직임 감지
        moveCheckTimer -= Time.deltaTime;
        if (isDark && moveCheckTimer <= 0f)
        {
            if (!hasInitializedLastPos)
            {
                lastPlayerPosition = player.position;
                hasInitializedLastPos = true;
                moveCheckTimer = 0.2f;
                return;
            }

            float moved = Vector3.Distance(player.position, lastPlayerPosition);
            if (moved > 0.2f) // 감지 민감도 완화
            {
                TriggerDeath();
                return;
            }

            lastPlayerPosition = player.position;
            moveCheckTimer = 0.2f;
        }

        if (!canMove || !isDark)
        {
            animator.SetBool("IsRunning", false);
            return;
        }

        animator.SetBool("IsRunning", true);

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionDistance)
        {
            TriggerDeath();
        }
    }

    public void SetCanMove(bool value) => canMove = value;

    public void SetDark(bool dark)
    {
        isDark = dark;

        // 어둠 시작 시 움직임 감지 초기화
        if (dark)
            hasInitializedLastPos = false;
    }

    public void PlaySwipe()
    {
        if (animator != null)
            animator.SetTrigger("SwipeNow");
    }

    public void OnSwipeHit()
    {
        if (!isKilling)
        {
            isKilling = true;
            if (playerDeath != null)
                playerDeath.StartDeath();
        }
    }

  private void TriggerDeath()
{
    if (isDark && !isKilling)
    {
        Vector3 playerBack = player.position - player.forward * 1.5f;
        playerBack.y = transform.position.y;
        transform.position = playerBack;

        Vector3 lookDir = (player.position - transform.position).normalized;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = targetRot;
        }
    }

    PlaySwipe();
}
}
