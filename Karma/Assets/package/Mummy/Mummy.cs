using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mummy : MonoBehaviour
{
    public int attackDamage = 1;
    public Transform initialPosition;
    public Transform player;
    private UnityEngine.AI.NavMeshAgent agent;

    public AudioSource footstepSource;
    public AudioClip attackSound;
    public AudioClip hissingSound;
    private AudioSource audioSource;
    public float stepInterval = 0.5f;


    private Animator animator;

    public float attackCooldown = 1f; // 공격 쿨타임 (초)
    private float lastAttackTime = -999f; // 마지막 공격 시간 저장

    private bool isChasing = false; // ▶ 추적 시작 여부
    void Awake()
    {
        if (initialPosition == null)
        {
            GameObject initPos = new GameObject("MummyInitialPosition");
            initPos.transform.position = transform.position;
            initPos.transform.rotation = transform.rotation;
            initialPosition = initPos.transform;

            Debug.LogWarning("initialPosition이 비어 있어 자동으로 현재 위치를 기준으로 생성했습니다.");
        }

        // ✅ 이 부분 추가
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }


    void Start()
    {
        transform.rotation = initialPosition.rotation;

        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();


        audioSource.loop = false; // 반복 재생 금지

        agent.isStopped = true; // ▶ 첫 시작에는 플레이어를 추적하지 않기
        animator.SetBool("isWalking", false); // ▶ 시작할 때 Idle 상태로
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("mummy_attack"))
        {
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
        }
        else
        {
            if (isChasing && player != null)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);

                if (agent.velocity.magnitude > 0.1f)
                {
                    animator.SetBool("isWalking", true);

                    if (!footstepSource.isPlaying)
                    {
                        footstepSource.Play(); // ❗ 한 번만 재생되게
                    }
                }
                else
                {
                    animator.SetBool("isWalking", false);

                    if (footstepSource.isPlaying)
                    {
                        footstepSource.Stop(); // ❗ 걷기 중단 시 즉시 정지
                    }
                }
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                footstepSource.Stop();

            }
        }
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }
        }
    }

    public void MummyChasing()
    {
        isChasing = true;

        audioSource.PlayOneShot(hissingSound);

        // 추적 시작 시 플레이어 방향으로 회전
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // 위아래 각도 제거
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }


    public void MummyReset()
    {
        isChasing = false;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.Warp(initialPosition.position);
        }
        else
        {
            transform.position = initialPosition.position;
        }

        transform.rotation = initialPosition.rotation; // 회전 복원 추가

        animator.SetBool("isWalking", false);


        Debug.Log("몬스터 초기화 완료 (위치 + 회전)");
    }
    public void DealDamage()
    {
        if (player != null)
        {
            PlayerHP health = player.GetComponent<PlayerHP>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
}