using UnityEngine;

public class BathroomLight : MonoBehaviour
{
    public GameObject[] targetObjects;

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
