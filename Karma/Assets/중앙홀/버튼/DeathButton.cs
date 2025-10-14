using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DeathButton : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactionDistance = 3f;
    public TextMeshProUGUI interactionText;
    public string promptMessage = "E를 눌러 버튼을 누르시겠습니까?";

    [Tooltip("버튼을 누른 후 다시 누를 수 있을 때까지의 대기 시간(초)")]
    public float cooldownTime = 3.0f;

    [Header("오디오 설정")]
    public AudioClip soundA;
    public AudioClip soundB;
    public AudioClip soundC;
    public AudioClip soundD;

    private bool isPlayerInRange = false;
    private bool isSequenceRunning = false;
    private Transform playerTransform;
    private AudioSource audioSource;

    private void OnEnable()
    {
        isSequenceRunning = false;
        Debug.Log("DeathButton 활성화됨. 상태 초기화.");
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance && !isSequenceRunning)
        {
            isPlayerInRange = true;
            if (interactionText != null)
            {
                interactionText.text = promptMessage;
                interactionText.gameObject.SetActive(true);
            }
        }
        else
        {
            isPlayerInRange = false;
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isSequenceRunning)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        isSequenceRunning = true;
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }

        StartCoroutine(DeathSequenceCoroutine());
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        Debug.Log("버튼 시퀀스 시작!");

        PlaySound(soundA, "'a' 소리 재생");
        PlaySound(soundB, "'b' 소리 재생");

        yield return new WaitForSeconds(1.2f);

        Debug.Log("1.2초 경과. 'c', 'd' 소리 재생.");
        PlaySound(soundC, "'c' 소리 재생");
        PlaySound(soundD, "'d' 소리 재생");

        if (GameManager.Instance != null)
        {
            // ✨ [수정] 리셋을 호출하기 전에, 스테이지를 1로 설정하는 코드를 추가합니다.
            Debug.Log($"현재 스테이지: {GameManager.Instance.stage} -> 1로 변경합니다.");
            GameManager.Instance.stage = 1;
            GameManager.Instance.ResetStage();
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다!");
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isSequenceRunning = false;
        Debug.Log("버튼 쿨타임 종료. 다시 사용 가능.");
    }

    private void PlaySound(AudioClip clip, string debugMessage)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log(debugMessage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}