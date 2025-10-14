using UnityEngine;

public class BathroomLight : MonoBehaviour
{
    public GameObject[] targetObjects;

    // ? [추가] GameManager가 호출할 수 있도록 조명을 리셋하는 public 함수를 만듭니다.
    public void ResetLights()
    {
        // 모든 조명 오브젝트를 비활성화(끄기)합니다.
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        Debug.Log("Bathroom lights have been reset.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
}