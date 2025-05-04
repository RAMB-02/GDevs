using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public GameObject[] targetObjects;       // ������ ������Ʈ��
    public float maxBlinkInterval = 0.5f;    // ������ ������ �ִ밪 (0 ~ �� �� ���̿��� ����)
    public float blinkDuration = 5f;         // ��ü ������ �ð�

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
