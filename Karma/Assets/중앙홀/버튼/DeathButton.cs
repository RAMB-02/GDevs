using UnityEngine;
using TMPro; // UI 텍스트를 사용하기 위해 추가

public class DeathButton : MonoBehaviour
{
    [Tooltip("플레이어가 상호작용할 수 있는 최대 거리")]
    public float interactionDistance = 3f;

    [Tooltip("상호작용 안내 UI 텍스트")]
    public TextMeshProUGUI interactionText;

    // [핵심 변경점] 인스펙터에서 설정할 수 있는 안내 문구 변수 추가
    [Tooltip("플레이어가 범위 안에 있을 때 표시될 안내 문구")]
    public string promptMessage = "E를 눌러 버튼을 누르시겠습니까?"; // 기본값 설정

    private bool isPlayerInRange = false;
    private Transform playerTransform;

    void Start()
    {
        // 시작할 때 플레이어의 Transform을 찾아 저장해둡니다.
        // 이 코드는 GameManager의 Awake() 이후에 실행되는 것이 좋으므로 Start()에 둡니다.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다! Player 오브젝트에 'Player' 태그가 있는지 확인해주세요.");
        }

        // 시작 시 안내 텍스트를 비활성화합니다. (보통 UI는 처음에 꺼져있습니다.)
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어를 찾지 못했으면 더 이상 진행하지 않습니다.
        if (playerTransform == null) return;

        // 플레이어와 버튼 사이의 거리를 계산합니다.
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 플레이어가 상호작용 가능한 거리 안에 있는지 확인합니다.
        if (distance <= interactionDistance)
        {
            isPlayerInRange = true;
            
            // [핵심 변경점] 인스펙터 변수(promptMessage)의 내용을 텍스트 UI에 표시
            if (interactionText != null)
            {
                interactionText.text = promptMessage; // 이제 인스펙터에서 설정한 값이 표시됩니다.
                if (!interactionText.gameObject.activeSelf) // 이미 활성화되어 있으면 다시 활성화할 필요 없음
                {
                    interactionText.gameObject.SetActive(true);
                }
            }
        }
        else // 플레이어가 범위 밖으로 나갔을 때
        {
            isPlayerInRange = false;
            // 거리가 멀어지면 안내 텍스트를 비활성화합니다.
            if (interactionText != null && interactionText.gameObject.activeSelf)
            {
                interactionText.gameObject.SetActive(false);
            }
        }

        // 플레이어가 범위 안에 있고 'E' 키를 눌렀을 때
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PressButton();
        }
    }

    // 버튼이 눌렸을 때 실행될 함수
    private void PressButton()
    {
        Debug.Log("버튼이 눌렸습니다! 플레이어가 죽습니다.");

        // GameManager의 ResetStage 함수를 호출하여 플레이어를 리셋(죽음 처리)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStage();
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다! GameManager 오브젝트가 씬에 있고 'GameManager' 스크립트가 붙어있는지 확인해주세요.");
        }
    }

    // (에디터용) 상호작용 범위를 시각적으로 보여주기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        if (transform != null) // 에디터에서 오브젝트가 선택 해제될 때 오류 방지
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}