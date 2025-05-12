using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public Transform spawnPoint;
    public int stage = 1;
    public int anomaly = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetStage();
        SetRandomAnomalies();
    }

    public void ResetStage()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = spawnPoint.position;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = spawnPoint.position;
            }

            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ResetVelocity();
            }

            Debug.Log("스폰 위치로 이동 완료");
        }
        else
        {
            Debug.LogWarning("Player 오브젝트를 찾지 못했습니다.");
        }

        // anomaly 오브젝트는 초기화만 (값 설정은 이후 단계에서)
        AnomalyManager.Instance.DeactivateAllAnomalies();
    }

    public void SetRandomAnomalies()
    {
        anomaly = AnomalyManager.Instance.RandomizeAnomalies();
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
