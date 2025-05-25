using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    public Image fadeImage;
    public float fadeTime = 0.5f;

    public IEnumerator Flash(System.Action onBlack)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        onBlack?.Invoke();
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / fadeTime);
            c.a = alpha;
            fadeImage.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}