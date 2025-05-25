using UnityEngine;

public class SpiderComeout : MonoBehaviour
{
    public GameObject targetObject;     // ��Ÿ�� �Ź� ������Ʈ
    public AudioClip soundEffect;       // ����� �Ҹ�
    public AudioSource audioSource;     // �Ҹ��� ����� ����� �ҽ�

    public float moveDuration = 0.5f;   // �̵� �� ȸ�� �ҿ� �ð�
    public float delayBeforeStart = 3f; // Ʈ���� �� ���۱��� ��� �ð�

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            StartCoroutine(DelayedActivation());
        }
    }

    private System.Collections.IEnumerator DelayedActivation()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        if (targetObject != null)
        {
            targetObject.SetActive(true);
            StartCoroutine(MoveAndRotateSpider(targetObject.transform));
        }

        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }

    private System.Collections.IEnumerator MoveAndRotateSpider(Transform target)
    {
        // �ʱⰪ
        Vector3 startPos = new Vector3(target.position.x, 10f, target.position.z);
        Vector3 endPos = new Vector3(target.position.x, 1f, target.position.z);

        Vector3 startRot = new Vector3(180f, target.eulerAngles.y, target.eulerAngles.z);
        Vector3 endRot = new Vector3(115f, target.eulerAngles.y, target.eulerAngles.z);

        float elapsed = 0f;

        target.position = startPos;
        target.eulerAngles = startRot;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            target.position = Vector3.Lerp(startPos, endPos, t);
            target.eulerAngles = Vector3.Lerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.position = endPos;
        target.eulerAngles = endRot;
    }
}
