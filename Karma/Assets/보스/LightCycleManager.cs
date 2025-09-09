using UnityEngine;

public class LightCycleManager : MonoBehaviour
{
    [Header("조명 설정")]
    public Light[] roomLights;
    public float interval = 3f;

    [Header("기능 연동")]
    public PlayerMoveCheck playerMoveCheck;
    public BossController bossController;
    public AudioSource tensionMusicSource;

    private float timer;
    private bool isLightOn = true;

    void OnEnable()
    {
        // 트리거로 활성화될 때 초기화
        timer = interval;
        isLightOn = true;

        foreach (Light light in roomLights)
            if (light != null) light.enabled = true;

        if (playerMoveCheck != null)
            playerMoveCheck.SetDark(false);

        if (bossController != null)
            bossController.SetDark(false);

        if (tensionMusicSource != null)
            tensionMusicSource.Stop();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isLightOn = !isLightOn;

            foreach (Light light in roomLights)
                if (light != null) light.enabled = isLightOn;

            if (playerMoveCheck != null)
                playerMoveCheck.SetDark(!isLightOn);

            if (bossController != null)
                bossController.SetDark(!isLightOn);

            if (tensionMusicSource != null)
            {
                if (!isLightOn && !tensionMusicSource.isPlaying)
                    tensionMusicSource.Play();
                else if (isLightOn && tensionMusicSource.isPlaying)
                    tensionMusicSource.Stop();
            }

            timer = interval;
        }
    }
}