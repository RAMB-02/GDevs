using UnityEngine;

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

    public StatueController statue; // Inspector에서 직접 연결

    public LightTrigger lightTrigger;

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

                // 저장된 위치가 있으면 그 위치로 이동, 없으면 기본 스폰지점으로 이동
                if (PlayerSpawnData.nextPosition != Vector3.zero)
                {
                    player.transform.position = PlayerSpawnData.nextPosition;
                    player.transform.rotation = PlayerSpawnData.nextRotation;

                    // 이동 후 초기화
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

        // 몬스터 및 기타 리셋
        if (monster != null)
        {
            monster.ResetToInitialPosition();
        }
        if (mummy != null)
        {
            mummy.MummyReset();
            mummytrig.OnEnable();
        }

        toilettrigger.OnEnable();

        // 조명 초기화
        LightManager.Instance.SetAnomalyLights(false);

        // 이상현상 초기화
        AnomalyManager.Instance.DeactivateAllAnomalies();

        if (lightTrigger != null)
        {
            lightTrigger.ResetTrigger();
        }
        if (statue != null)
        {
            Debug.Log("조각상 리셋!");
            statue.ResetStatue();
        }
        else
        {
            Debug.LogWarning("Statue가 GameManager에 연결되지 않았습니다!");
        }
        AnomalyManager.Instance.ResetAnomalyTriggers();
    }

    public void SetRandomAnomalies()
    {
        anomaly = AnomalyManager.Instance.RandomizeAnomalies();
        Debug.Log("이상현상 수: " + anomaly + " 스테이지: " + stage);
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
