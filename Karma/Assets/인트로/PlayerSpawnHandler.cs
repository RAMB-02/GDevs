using UnityEngine;
using System.Collections;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ApplySavedPosition());
    }

    private IEnumerator ApplySavedPosition()
    {
        // 씬 전환 직후 잠깐 대기 (위치 세팅 안정화용)
        yield return new WaitForSeconds(0.1f);

        if (PlayerSpawnData.nextPosition != Vector3.zero)
        {
            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;

            transform.position = PlayerSpawnData.nextPosition;
            transform.rotation = PlayerSpawnData.nextRotation;

            if (cc != null) cc.enabled = true;

            // 위치 적용 후 초기화
            PlayerSpawnData.nextPosition = Vector3.zero;
            PlayerSpawnData.nextRotation = Quaternion.identity;
        }
    }
}