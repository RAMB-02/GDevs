using UnityEngine;

public class TestScript : MonoBehaviour
{
    // 인스펙터에서 드래그 앤 드롭으로 연결할 수 있는 오브젝트
    public GameObject singleObject; // 하나만 넣는 경우
    public GameObject[] multipleObjects; // 여러 개 넣는 경우

    void Start()
    {
        // 연결된 오브젝트 확인
        if (singleObject != null)
        {
            Debug.Log("싱글 오브젝트: " + singleObject.name);
        }

        if (multipleObjects != null && multipleObjects.Length > 0)
        {
            Debug.Log("여러 오브젝트들:");
            foreach (GameObject obj in multipleObjects)
            {
                Debug.Log(obj.name);
            }
        }
    }
}
