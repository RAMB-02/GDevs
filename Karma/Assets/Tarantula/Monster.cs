using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public Transform player;
    private UnityEngine.AI.NavMeshAgent agent;

    public AudioClip footstepSound;
    public AudioClip attackSound;
    private AudioSource audioSource;
    public float stepInterval = 0.5f;
    private float stepTimer = 0f;

    private Animator animator;

    public float attackCooldown = 1f; // 공격 쿨타임 (초)
    private float lastAttackTime = -999f; // 마지막 공격 시간 저장

    private bool isChasing = false; // ▶ 추적 시작 여부

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.isStopped = true; // ▶ 첫 시작에는 플레이어를 추적하지 않기
        animator.SetBool("isWalking", false); // ▶ 시작할 때 Idle 상태로
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("rig|rig|attack_01"))
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
                    stepTimer += Time.deltaTime;
                    if (stepTimer >= stepInterval)
                    {
                        audioSource.PlayOneShot(footstepSound);
                        stepTimer = 0f;
                    }
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    stepTimer = 0f;
                }
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                stepTimer = 0f;
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

    public void StartChasing() // ▶ 특정 명령에 의해 추적 시작
    {
        isChasing = true;
    }
}
