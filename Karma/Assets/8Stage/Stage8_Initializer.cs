using UnityEngine;

public class Stage8_Initializer : MonoBehaviour
{
    void Awake()
    {
        // 8스테이지가 시작될 때, '목걸이를 훔친 상태'로 강제 설정합니다.
        NecklessScript.hasStolenNecklace = true;
    }
}