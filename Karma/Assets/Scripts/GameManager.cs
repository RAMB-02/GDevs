using UnityEngine;
using System.Collections; // Coroutine을 사용하기 위해 추가
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Monster monster;
    public Mummy mummy;
    public MummyTrigger mummytrig;
    public ToiletTrigger toilettrigger;

    [Header("Settings")]
    public Transform spawnPoint;
    public int stage = 1;
    public int anomaly = 0;

    [Header("UI")]
    public TextMeshProUGUI worldStageText;

    public StatueController statue;
    public LightTrigger lightTrigger; // 인스펙터에서 연결 필수

    // GameManager가 조명 상태를 직접 제어하는 함수
    public void SetAnomalyLightState(bool isAnomaly)
    {
        if (LightManager.Instance != null)
        {
            LightManager.Instance.SetAnomalyLights(isAnomaly);
        }
        else
        {
            Debug.LogError("LightManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetStage();
        SetRandomAnomalies();
    }

    void UpdateStageUI()
    {
        if (worldStageText != null)
        {
            worldStageText.text = stage.ToString(); //stage를 문자열로 변환해서 텍스트로 표시 
        }
    }


    public void ResetStage()
    {
        // ------------------------------------
        // LightTrigger를 일시적으로 비활성화
        // ------------------------------------
        if (lightTrigger != null)
        {
            lightTrigger.gameObject.SetActive(false);
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;

                if (PlayerSpawnData.nextPosition != Vector3.zero)
                {
                    player.transform.position = PlayerSpawnData.nextPosition;
                    player.transform.rotation = PlayerSpawnData.nextRotation;
                    PlayerSpawnData.nextPosition = Vector3.zero;
                    PlayerSpawnData.nextRotation = Quaternion.identity;
                }
                else
                {
                    player.transform.position = spawnPoint.position;
                }

                cc.enabled = true;
            }
            else
            {
                if (PlayerSpawnData.nextPosition != Vector3.zero)
                {
                    player.transform.position = PlayerSpawnData.nextPosition;
                    player.transform.rotation = PlayerSpawnData.nextRotation;
                    PlayerSpawnData.nextPosition = Vector3.zero;
                    PlayerSpawnData.nextRotation = Quaternion.identity;
                }
                else
                {
                    player.transform.position = spawnPoint.position;
                }
            }

            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ResetVelocity();
            }

            Debug.Log("플레이어 위치 리셋 완료");
        }
        else
        {
            Debug.LogWarning("Player 태그가 있는 오브젝트를 찾을 수 없습니다!");
        }

        // ------------------------------------
        // 게임 오브젝트 상태 초기화
        // ------------------------------------

        // 조명 초기화 (GameManager가 직접 관리)
        SetAnomalyLightState(false);
        if (lightTrigger != null)
        {
            lightTrigger.ResetTrigger(); // LightTrigger의 상태만 초기화
        }

        // 몬스터 및 기타 초기화
        if (monster != null)
            monster.ResetToInitialPosition();

        if (mummy != null)
        {
            mummy.MummyReset();
        }
        if (mummytrig != null)
        {
            mummytrig.OnEnable();
        }
        if (toilettrigger != null)
        {
            toilettrigger.OnEnable();
        }

        // 이상현상 초기화
        if (AnomalyManager.Instance != null)
        {
            AnomalyManager.Instance.DeactivateAllAnomalies();
            AnomalyManager.Instance.ResetAnomalyTriggers();
        }

        // 조각상 초기화
        if (statue != null)
        {
            Debug.Log("조각상 리셋!");
            statue.ResetStatue();
        }
        else
        {
            Debug.LogWarning("Statue가 GameManager에 연결되지 않았습니다!");
        }

        // ------------------------------------
        // 모든 리셋 작업 후 LightTrigger 다시 활성화
        // ------------------------------------
        if (lightTrigger != null)
        {
            lightTrigger.gameObject.SetActive(true);
        }

        UpdateStageUI();
    }

    public void SetRandomAnomalies()
    {
        anomaly = AnomalyManager.Instance.RandomizeAnomalies();
        Debug.Log("이상현상 수: " + anomaly + " 스테이지: " + stage);
        UpdateStageUI();
    }

    public void MoveToBackDoor()
    {
        if (anomaly >= 1)
            stage++;
        else
            stage = 1;

        ResetStage();
        SetRandomAnomalies();
    }

    public void MoveToFrontDoor()
    {
        if (anomaly >= 1)
            stage = 1;
        else
            stage++;

        ResetStage();
        SetRandomAnomalies();
    }
}