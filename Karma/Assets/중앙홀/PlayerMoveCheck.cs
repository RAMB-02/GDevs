using UnityEngine;
public class PlayerMoveCheck : MonoBehaviour
{
    public BossController boss;
    public bool isDark = false;
    Vector3 lastPosition;
    float moveThreshold = 0.05f;

    void Update()
    {
        if (isDark)
        {
            float moved = Vector3.Distance(transform.position, lastPosition);
            if (moved > moveThreshold)
            {
                boss.PlaySwipe();
            }
        }
        lastPosition = transform.position;
    }

    public void SetDark(bool state)
    {
        isDark = state;
    }
}