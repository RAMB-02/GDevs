using UnityEngine;
using UnityEngine.AI; // NavMesh Agent를 사용하기 위해 필수!

[RequireComponent(typeof(NavMeshAgent))] // 이 스크립트는 NavMeshAgent가 반드시 필요함을 명시
public class StatueController : MonoBehaviour
{
    [Header("대상 및 범위 설정")]
    public Transform player;
    public float chaseRange = 10f;

    [Header("오디오 설정")]
    public AudioSource moveSound;

    [Header("스폰 위치")]
    public Transform spawnPoint;

    // --- 내부 변수들 ---
    private Renderer statueRenderer;
    private Camera playerCamera;
    private NavMeshAgent agent;

    // [핵심 수정 1] 컴포넌트 초기화는 Start()보다 항상 먼저 실행되는 Awake()에서 처리합니다.
    // 이렇게 해야 다른 스크립트가 Start()에서 이 스크립트를 호출해도 에러가 나지 않습니다.
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        statueRenderer = GetComponentInChildren<Renderer>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (player == null) return; // 플레이어가 없으면 아무것도 하지 않음

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            // 플레이어가 화면으로 석상을 보고 있는지 확인
            if (IsVisibleToCamera())
            {
                // 보고 있다면: 그 자리에 멈춤
                agent.isStopped = true;
                PlayMoveSound(false);
            }
            else
            {
                // 보고 있지 않다면: 플레이어를 향해 이동
                agent.isStopped = false;
                MoveTowardsPlayer();
                PlayMoveSound(true);
            }
        }
        else
        {
            // 추적 범위 밖이면 멈춤
            agent.isStopped = true;
            PlayMoveSound(false);
        }
    }

    // 플레이어 카메라에 석상이 보이는지 정밀하게 확인하는 함수
    bool IsVisibleToCamera()
    {
        // 1. 카메라 시야각(절두체) 안에 있는지 기본적인 확인
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        if (!GeometryUtility.TestPlanesAABB(planes, statueRenderer.bounds))
        {
            return false; // 시야각 밖에 있으면 무조건 안 보이는 것
        }

        // 2. 카메라와 석상 사이에 다른 벽이 있는지 광선(Raycast)으로 확인
        Vector3 directionToStatue = (statueRenderer.bounds.center - playerCamera.transform.position).normalized;
        float distanceToCenter = Vector3.Distance(playerCamera.transform.position, statueRenderer.bounds.center);

        if (Physics.Raycast(playerCamera.transform.position, directionToStatue, out RaycastHit hit, distanceToCenter))
        {
            // 광선이 석상보다 먼저 다른 것에 부딪혔다면, 벽에 가려진 것
            if (hit.transform != transform && !hit.transform.IsChildOf(transform))
            {
                return false;
            }
        }

        // 모든 관문을 통과하면 보이는 것
        return true;
    }

    // [핵심 수정 2] NavMesh Agent를 사용해 길을 따라 이동하도록 변경 (벽 뚫기 방지)
    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    // 이동 소리를 켜고 끄는 함수
    void PlayMoveSound(bool play)
    {
        if (moveSound == null) return;

        if (play && !moveSound.isPlaying)
        {
            moveSound.Play();
        }
        else if (!play && moveSound.isPlaying)
        {
            moveSound.Stop();
        }
    }

    // 석상을 스폰 위치로 리셋하는 함수
    public void ResetStatue()
    {
        if (spawnPoint != null)
        {
            // [핵심 수정 3] NavMeshAgent의 위치를 옮길 때는 Warp()를 사용해야 안전합니다.
            agent.Warp(spawnPoint.position);
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("Statue의 SpawnPoint가 설정되어 있지 않습니다.");
        }
    }

    // 플레이어와 충돌(Trigger)했을 때 처리하는 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("조각상이 플레이어와 충돌했습니다. 스테이지 리셋!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.stage = 1; // 필요하다면 스테이지 변수 조절
                GameManager.Instance.ResetStage();
            }
        }
    }
}