using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFadeManager : MonoBehaviour
{
    public static SceneFadeManager I;

    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.35f;

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(FadeTo(0f));
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutLoadFadeIn(sceneName));
    }

    IEnumerator FadeOutLoadFadeIn(string sceneName)
    {
        yield return FadeTo(1f);
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return FadeTo(0f);
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float start = fadeGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
    }
}
