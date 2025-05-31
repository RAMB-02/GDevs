using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerHP : MonoBehaviour
{
    private LensDistortion lensDistortion;
    public float healDelay = 5f;        // 마지막 피해 이후 회복 시작까지 기다리는 시간
    public float healInterval = 1f;     // 회복 주기
    public int healAmount = 1;          // 회복량
    private float lastHitTime;          // 마지막으로 피해 입은 시간
    private float healTimer = 0f;       // 회복 주기 타이머

    public CameraShake cameraShake;
    public AudioClip[] hitSounds;
    private AudioSource audioSource;
    public int maxHealth = 3;
    private int currentHealth;

    public Volume volume;
    private ChromaticAberration chroma;


    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (volume.profile.TryGet(out chroma) && volume.profile.TryGet(out lensDistortion)) 
        {
            UpdateVisualEffect();
        }
    }

    void Update()
    {
        TryHeal();
    }

    void TryHeal()
    {
        // 체력이 이미 최대거나, 마지막으로 맞은지 healDelay가 지나지 않았으면 패스
        if (currentHealth >= maxHealth || Time.time - lastHitTime < healDelay)
            return;

        // 주기적으로 체력 회복
        healTimer += Time.deltaTime;
        if (healTimer >= healInterval)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            healTimer = 0f;
            UpdateVisualEffect(); // 회복에 따라 시각 효과도 조정
        }
    }

    


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        lastHitTime = Time.time;
        UpdateVisualEffect();

        if (cameraShake != null)
        {
            cameraShake.Shake();
        }

        if (hitSounds != null && hitSounds.Length > 0 && audioSource != null)
        {
            int index = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[index], 1.0f);
        }



        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateVisualEffect()
    {
        float ratio = (float)currentHealth / maxHealth;

        float effectStrength = Mathf.Pow(1f - ratio, 0.7f);

        if (chroma != null)
        {
            chroma.intensity.value = Mathf.Lerp(0f, 1.5f, effectStrength);
        }

        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.5f, effectStrength);
        }
}

    private void Die()
    {
        Debug.Log("플레이어 사망");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStage();
            GameManager.Instance.SetRandomAnomalies();

            currentHealth = maxHealth;
            UpdateVisualEffect();
        }
    }
}
