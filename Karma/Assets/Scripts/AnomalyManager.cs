using UnityEngine;
using System.Collections.Generic;
public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("Anomaly Settings")]
    public List<GameObject> anomalyObjects;
    [Range(0f, 1f)] public float activationChance = 0.3f;

    [Header("Spider Monster Object")]
    public GameObject monsterObject;
    private Monster monster;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (monsterObject != null)
        {
            monster = monsterObject.GetComponent<Monster>();
            if (monster == null)
            {
                Debug.LogError("Monster script not found on assigned object!");
            }
        }
    }

    public void DeactivateAllAnomalies()
    {
        foreach (GameObject obj in anomalyObjects)
        {
            obj.SetActive(false);
        }


    }

    public int RandomizeAnomalies()
    {
        int count = 0;

        foreach (GameObject obj in anomalyObjects)
        {
            bool activate = Random.value < activationChance;
            obj.SetActive(activate);

            if (activate) count++;
        }

        if (monsterObject != null && monster != null)
        {
            bool activate = Random.value < activationChance;
            monsterObject.SetActive(activate);

            if (activate)
            {
                monster.StartChasing();
                count++;
            }
        }

        return count;
    }
}