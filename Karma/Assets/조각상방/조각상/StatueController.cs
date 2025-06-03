using UnityEngine;

public class StatueController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public AudioSource moveSound;

    public Transform spawnPoint; // ✅ 스폰 위치 오브젝트 연결 (Inspector에서 드래그)

    private Renderer statueRenderer;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        statueRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
            PlayMoveSound(false);
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
            return hit.transform == statueRenderer.transform || hit.transform.IsChildOf(transform);
        }

        return true;
    }

    void MoveTowardsPlayer()
    {
        Vector3 targetPosition = player.position;
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

    // ✅ 스폰 위치 오브젝트 위치로 되돌리기
    public void ResetStatue()
    {
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("Statue의 SpawnPoint가 설정되어 있지 않습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("조각상이 플레이어와 충돌했습니다. 스테이지 리셋!");
            GameManager.Instance.stage = 1;
            GameManager.Instance.ResetStage();
            GameManager.Instance.SetRandomAnomalies();
        }
    }
}
