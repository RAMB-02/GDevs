using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class NecklessScript : MonoBehaviour
{
    // static 변수를 사용하여 게임 내 어디서든 이 값을 공유할 수 있습니다.
    public static bool hasStolenNecklace = false;

    // ? 새로 추가된 신호 전달용 변수
    public static bool playSoundOnNextSceneLoad = false;


    [Header("대상 오브젝트")]
    public GameObject necklaceObject;

    [Header("사운드")]
    public AudioSource clickSound;

    [Header("UI")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText; // UI 텍스트를 변경하기 위한 변수 (TMP용)

    private bool isPlayerInRange = false;

    void Start()
    {
        // 게임 시작 시, 도난 상태에 따라 목걸이의 초기 상태를 설정합니다.
        if (necklaceObject != null)
        {
            necklaceObject.SetActive(!hasStolenNecklace);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 현재 도난 상태를 반전시킵니다.
            hasStolenNecklace = !hasStolenNecklace;

            // 상태에 따라 목걸이를 보이거나 숨깁니다.
            if (necklaceObject != null)
            {
                necklaceObject.SetActive(!hasStolenNecklace);
            }

            // 소리를 재생합니다.
            if (clickSound != null)
            {
                clickSound.Play();
            }

            // UI 텍스트를 현재 상태에 맞게 업데이트합니다.
            UpdateInteractionUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                UpdateInteractionUI(); // 플레이어가 범위에 들어왔을 때 UI 텍스트 업데이트
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    // UI 텍스트를 업데이트하는 함수
    private void UpdateInteractionUI()
    {
        if (interactionText != null)
        {
            if (hasStolenNecklace)
            {
                interactionText.text = "목걸이 되돌려놓기(E)";
            }
            else
            {
                interactionText.text = "목걸이 훔치기(E)";
            }
        }
    }
}