using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public GameObject[] targetObjects;       // 깜빡일 오브젝트들
    public float maxBlinkInterval = 0.5f;    // 깜빡임 간격의 최대값 (0 ~ 이 값 사이에서 랜덤)
    public float blinkDuration = 5f;         // 전체 깜빡임 시간

    private bool isBlinking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBlinking)
        {
            StartCoroutine(BlinkLights());
        }
    }

    private System.Collections.IEnumerator BlinkLights()
    {
        isBlinking = true;
        float timer = 0f;

        while (timer < blinkDuration)
        {
            ToggleAllObjects();

            float randomInterval = Random.Range(0f, maxBlinkInterval);
            yield return new WaitForSeconds(randomInterval);
            timer += randomInterval;
        }

        SetAllObjectsActive(true);
        isBlinking = false;
    }

    private void ToggleAllObjects()
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }

    private void SetAllObjectsActive(bool active)
    {
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
        }
    }
}
