// LightOnOff.cs

using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public GameObject[] targetObjects;
    public float maxBlinkInterval = 0.5f;
    public float blinkDuration = 5f;

    private bool isBlinking = false;

    // --- ? [추가] 리셋 함수 ---
    // 외부(GameManager)에서 이 함수를 호출하여 코루틴을 강제 종료하고 조명을 켭니다.
    public void ResetBlinking()
    {
        StopAllCoroutines();      // 1. 진행 중인 모든 코루틴 (BlinkLights)을 즉시 중단합니다.
        SetAllObjectsActive(true); // 2. 모든 조명 오브젝트를 활성화(ON) 상태로 강제 설정합니다.
        isBlinking = false;        // 3. 내부 상태를 초기화하여 다시 트리거될 수 있도록 합니다.
    }

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