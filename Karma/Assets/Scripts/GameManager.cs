using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ... (기존 변수들은 모두 동일)
    public static GameManager Instance;
    public BathroomLight bathroomLight;
    public Monster monster;
    public Mummy mummy;
    public MummyTrigger mummytrig;
    public EyeController2D eyeController;
    public PlayerDetection eyetrig;
    public ToiletTrigger toilettrigger;
    public StatueController statue;
    public OiioiCatController oiioiCat;

    [Header("Settings")]
    public Transform spawnPoint;
    public int stage = 1;
    public int anomaly = 0;

    [Header("UI")]
    public TextMeshProUGUI worldStageText;
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    public LightTrigger lightTrigger;
    private Collider lightTriggerCollider;
    private bool isResetting = false;

    // ... (Awake, Start, SetAnomalyLightState, UpdateStageUI, ResetStage, ResetStageWithFade 함수는 모두 동일)
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
        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.gameObject.SetActive(false);
        }
        ResetStage();
    }

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

    void UpdateStageUI()
    {
        if (worldStageText != null)
        {
            worldStageText.text = stage.ToString();
        }
    }

    public void ResetStage(bool withFade = true)
    {
        if (isResetting) return;

        if (withFade)
        {
            StartCoroutine(ResetStageWithFade());
        }
        else
        {
            ResetStageLogic();
        }
    }

    private IEnumerator ResetStageWithFade()
    {
        isResetting = true;
        yield return StartCoroutine(FadeToBlack());
        ResetStageLogic();
        yield return StartCoroutine(FadeToClear());
        isResetting = false;
    }

    private void ResetStageLogic()
    {
        if (lightTriggerCollider != null) lightTriggerCollider.enabled = false;

        // ✨ [수정] 씬에 있는 모든 SoundTrigger를 찾아서 리셋하는 로직 추가
        SoundTrigger[] soundTriggers = FindObjectsOfType<SoundTrigger>();
        foreach (SoundTrigger trigger in soundTriggers)
        {
            trigger.ResetTrigger();
        }
        Debug.Log($"씬에 있는 {soundTriggers.Length}개의 SoundTrigger를 리셋했습니다.");


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

        LightOnOff[] lightBlinkers = FindObjectsOfType<LightOnOff>();
        foreach (LightOnOff blinker in lightBlinkers)
        {
            blinker.ResetBlinking();
        }

        if (lightTrigger != null) lightTrigger.ResetTrigger();
        if (monster != null) monster.ResetToInitialPosition();
        if (mummy != null) mummy.MummyReset();
        if (mummytrig != null) mummytrig.OnEnable();
        if (toilettrigger != null) toilettrigger.OnEnable();
        if (eyeController != null)
        {
            eyeController.StopTracking();
            eyetrig.OnEnable();
        }

        if (AnomalyManager.Instance != null)
        {
            AnomalyManager.Instance.DeactivateAllAnomalies();
            AnomalyManager.Instance.ResetAnomalyTriggers();
        }
        if (statue != null) statue.ResetStatue();
        if (oiioiCat != null)
        {
            oiioiCat.ResetCat();
        }

        UpdateStageUI();
        SetRandomAnomalies();
        SetAnomalyLightState(false);

        if (bathroomLight != null)
        {
            bathroomLight.ResetLights();
        }

        if (lightTriggerCollider != null) lightTriggerCollider.enabled = true;
    }

    // ... (나머지 함수들은 모두 동일)
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
        ResetStage(false);
    }

    public void MoveToFrontDoor()
    {
        if (anomaly >= 1) stage = 1;
        else stage++;
        ResetStage(false);
    }

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;
        fadePanel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, Mathf.Clamp01(elapsedTime / fadeDuration));
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeToClear()
    {
        if (fadePanel == null) yield break;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, 1f - Mathf.Clamp01(elapsedTime / fadeDuration));
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false);
    }
}