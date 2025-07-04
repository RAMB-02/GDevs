using UnityEngine;
using System.Collections.Generic;
public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("Anomaly Settings")]
    public List<GameObject> anomalyObjects;
    [Range(0f, 1f)] public float activationChance = 0.3f;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    public void DeactivateAllAnomalies()
    {
        foreach (GameObject obj in anomalyObjects)
        {
            obj.SetActive(false);
        }
    }
    public void ResetAnomalyTriggers()
    {
        foreach (GameObject obj in anomalyObjects)
        {
            var hideTrigger = obj.GetComponent<HideOnTrigger>();
            if (hideTrigger != null)
                hideTrigger.ResetTrigger();

            var timedTrigger = obj.GetComponent<TimedActivationTrigger>();
            if (timedTrigger != null)
                timedTrigger.ResetTrigger();

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


        return count;
    }
}