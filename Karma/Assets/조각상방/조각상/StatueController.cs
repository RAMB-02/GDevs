using UnityEngine;

public class StatueController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float chaseRange = 10f; // ✅ 최대 추적 거리 추가
    public AudioSource moveSound;

    Renderer statueRenderer;
    Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        statueRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ✅ 일정 거리 이내일 때만 행동
        if (distanceToPlayer <= chaseRange)
        {
            if (!IsVisibleToCamera())
            {
                MoveTowardsPlayer();
                PlayMoveSound(true);
            }
            else
            {
                PlayMoveSound(false);
            }
        }
        else
        {
            PlayMoveSound(false); // 거리가 멀면 소리도 꺼짐
        }
    }

    bool IsVisibleToCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        if (!GeometryUtility.TestPlanesAABB(planes, statueRenderer.bounds))
            return false;

        Vector3 dirToStatue = (statueRenderer.bounds.center - playerCamera.transform.position).normalized;
        float distance = Vector3.Distance(playerCamera.transform.position, statueRenderer.bounds.center);

        if (Physics.Raycast(playerCamera.transform.position, dirToStatue, out RaycastHit hit, distance))
        {
            if (hit.transform == statueRenderer.transform || hit.transform.IsChildOf(transform))
                return true;

            return false;
        }

        return true;
    }

    void MoveTowardsPlayer()
    {
        Vector3 targetPosition = player.position; // y좌표를 고정하지 않음
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void PlayMoveSound(bool isMoving)
    {
        if (isMoving && !moveSound.isPlaying)
            moveSound.Play();
        else if (!isMoving && moveSound.isPlaying)
            moveSound.Stop();
    }
}
