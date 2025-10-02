// GameManager.cs

using UnityEngine;
using System.Collections;
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
    public LightTrigger lightTrigger;

    private Collider lightTriggerCollider;

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
            if (lightTrigger != null)
            {
                lightTriggerCollider = lightTrigger.GetComponent<Collider>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetStage();
    }

    void UpdateStageUI()
    {
        if (worldStageText != null)
        {
            worldStageText.text = stage.ToString();
        }
    }

    public void ResetStage()
    {
        // 1. LightTrigger의 물리 감지를 먼저 끕니다.
        if (lightTriggerCollider != null)
        {
            lightTriggerCollider.enabled = false;
        }

        // 2. 플레이어 위치를 리셋합니다.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = (PlayerSpawnData.nextPosition != Vector3.zero) ? PlayerSpawnData.nextPosition : spawnPoint.position;
                player.transform.rotation = (PlayerSpawnData.nextPosition != Vector3.zero) ? PlayerSpawnData.nextRotation : Quaternion.identity;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = (PlayerSpawnData.nextPosition != Vector3.zero) ? PlayerSpawnData.nextPosition : spawnPoint.position;
                player.transform.rotation = (PlayerSpawnData.nextPosition != Vector3.zero) ? PlayerSpawnData.nextRotation : Quaternion.identity;
            }
            PlayerSpawnData.nextPosition = Vector3.zero;
            PlayerSpawnData.nextRotation = Quaternion.identity;
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.ResetVelocity();
            Debug.Log("플레이어 위치 리셋 완료");
        }

        // --- ✅ [수정] 모든 LightOnOff 트리거를 찾아 리셋하는 코드 추가 ---
        // 씬에 있는 모든 LightOnOff 스크립트를 찾아서 강제로 초기화시킵니다.
        // 이것이 깜빡이던 조명을 원래 상태로 되돌리는 핵심 코드입니다.
        LightOnOff[] lightBlinkers = FindObjectsOfType<LightOnOff>();
        foreach (LightOnOff blinker in lightBlinkers)
        {
            blinker.ResetBlinking();
        }

        // 3. 몬스터 및 다른 트리거들을 리셋합니다.
        if (lightTrigger != null) lightTrigger.ResetTrigger();
        if (monster != null) monster.ResetToInitialPosition();
        if (mummy != null) mummy.MummyReset();
        if (mummytrig != null) mummytrig.OnEnable();
        if (toilettrigger != null) toilettrigger.OnEnable();
        if (AnomalyManager.Instance != null)
        {
            AnomalyManager.Instance.DeactivateAllAnomalies();
            AnomalyManager.Instance.ResetAnomalyTriggers();
        }
        if (statue != null) statue.ResetStatue();

        // 4. UI를 업데이트하고 새로운 이상현상을 설정합니다.
        UpdateStageUI();
        SetRandomAnomalies();

        // BUG FIX: 모든 이상현상 설정이 끝난 후, 조명 상태를 '정상'으로 최종 확정합니다.
        SetAnomalyLightState(false);

        // 5. 모든 작업이 끝난 후 LightTrigger의 물리 감지를 다시 켭니다.
        if (lightTriggerCollider != null)
        {
            lightTriggerCollider.enabled = true;
        }
    }

    public void SetRandomAnomalies()
    {
        if (AnomalyManager.Instance != null)
        {
            anomaly = AnomalyManager.Instance.RandomizeAnomalies();
            Debug.Log("이상현상 수: " + anomaly + " 스테이지: " + stage);
            UpdateStageUI();
        }
        else
        {
            Debug.LogError("AnomalyManager 인스턴스가 없습니다!");
        }
    }

    public void MoveToBackDoor()
    {
        if (anomaly >= 1) stage++;
        else stage = 1;
        ResetStage();
    }

    public void MoveToFrontDoor()
    {
        if (anomaly >= 1) stage = 1;
        else stage++;
        ResetStage();
    }
}