using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferTrigger : MonoBehaviour
{
    public string targetSceneName = "MainGameScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawnData.nextPosition = other.transform.position;
            PlayerSpawnData.nextRotation = other.transform.rotation;

            FadeManager.Instance.FadeToScene(targetSceneName);
        }
    }
}
