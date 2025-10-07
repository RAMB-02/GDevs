using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public EyeController2D eyeController; 

    private bool triggered = false;

    public void OnEnable()
    {
        triggered = false; 
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            eyeController.StartTracking();
            //StartCoroutine(EndChaseAfterDelay());
        }
    }
}