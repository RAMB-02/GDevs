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

            Debug.Log("���� ��ġ�� �̵� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("Player ������Ʈ�� ã�� ���߽��ϴ�.");
        }

        // �� ���� ����
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

        AnomalyManager.Instance.DeactivateAllAnomalies();
    }


    public void SetRandomAnomalies()
    {
        anomaly = AnomalyManager.Instance.RandomizeAnomalies();
        Debug.Log("�̻����� ��: " + anomaly + "��������: " + stage);


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