using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            LightManager.Instance.SetAnomalyLights(true);
            triggered = true;
        }
    }

    public void ResetTrigger()
    {
        triggered = false;
        LightManager.Instance.SetAnomalyLights(false); // ���� ����ȭ
    }

    private void OnEnable()
    {
        ResetTrigger(); // �߿�: ������Ʈ�� �ٽ� Ȱ��ȭ�� �� �ʱ�ȭ
    }
}