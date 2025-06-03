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
        // �� ��ȯ ���� ��� ��� (��ġ ���� ����ȭ��)
        yield return new WaitForSeconds(0.1f);

        if (PlayerSpawnData.nextPosition != Vector3.zero)
        {
            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;

            transform.position = PlayerSpawnData.nextPosition;
            transform.rotation = PlayerSpawnData.nextRotation;

            if (cc != null) cc.enabled = true;

            // ��ġ ���� �� �ʱ�ȭ
            PlayerSpawnData.nextPosition = Vector3.zero;
            PlayerSpawnData.nextRotation = Quaternion.identity;
        }
    }
}