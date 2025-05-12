using UnityEngine;

public class SpiderComeout : MonoBehaviour
{
    public GameObject targetObject;     // 나타날 거미 오브젝트
    public AudioClip soundEffect;       // 재생할 소리
    public AudioSource audioSource;     // 소리를 재생할 오디오 소스

    public float moveDuration = 0.5f;   // 이동 및 회전 소요 시간
    public float delayBeforeStart = 3f; // 트리거 후 시작까지 대기 시간

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
        // 초기값
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
