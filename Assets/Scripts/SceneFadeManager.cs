using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneFadeManager : MonoBehaviour
{
    public static SceneFadeManager I;
    [Header("Assign in Inspector")]
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
        // Safety defaults (prevents invisible click blocking)
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1f;
            fadeGroup.interactable = false;
            fadeGroup.blocksRaycasts = true; // while fading in from black
        }
    }
    void Start()
    {
        StartCoroutine(FadeTo(0f)); // fade in on start
    }
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutLoadFadeIn(sceneName));
    }
    IEnumerator FadeOutLoadFadeIn(string sceneName)
    {
        yield return FadeTo(1f);        // fade OUT (block clicks)
        SceneManager.LoadScene(sceneName);
        yield return null;              // wait 1 frame
        yield return FadeTo(0f);        // fade IN (release clicks)
    }
    public void FadeOut()
    {
        StartCoroutine(FadeTo(1f));
    }
    public void FadeIn()
    {
        StartCoroutine(FadeTo(0f));
    }
    IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeGroup == null) yield break;
        // If we are visible at all, block clicks.
        fadeGroup.blocksRaycasts = true;
        fadeGroup.interactable = false;
        float start = fadeGroup.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            yield return null;
        }
        fadeGroup.alpha = targetAlpha;
        // ✅ CRITICAL: when fully transparent, DO NOT block clicks
        fadeGroup.blocksRaycasts = targetAlpha > 0.01f;
    }
}