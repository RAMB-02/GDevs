using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    public GameObject bloodScreenEffect;
    public float restartDelay = 3f;
    private Image bloodImage;
    private bool isDying = false;
    private float alpha = 0f;

    void Start()
    {
        if (bloodScreenEffect != null)
        {
            bloodImage = bloodScreenEffect.GetComponent<Image>();
            bloodImage.color = new Color(1, 0, 0, 0); // 투명한 빨간색
            bloodScreenEffect.SetActive(false);
        }
    }

    public void StartDeath()
    {
        if (isDying) return;

        isDying = true;

        if (bloodScreenEffect != null)
        {
            bloodScreenEffect.SetActive(true);
            alpha = 0f;
        }

        Invoke(nameof(RestartScene), restartDelay);
    }

    void Update()
    {
        if (isDying && bloodImage != null && alpha < 0.8f)
        {
            alpha += Time.deltaTime * 0.5f;
            bloodImage.color = new Color(1, 0, 0, alpha);
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}