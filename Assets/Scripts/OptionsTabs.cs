using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsTabs : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Canvas Groups")]
    public CanvasGroup settingsGroup;
    public CanvasGroup controlsGroup;

    [Header("Tab Buttons")]
    public Button settingsTabButton;
    public Button controlsTabButton;

    [Header("Tab Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1, 1, 1, 0.6f);

    [Header("Fade")]
    public float fadeDuration = 0.25f;

    Image settingsTabImage;
    Image controlsTabImage;
    Coroutine fadeRoutine;

    void Awake()
    {
        settingsTabImage = settingsTabButton.GetComponent<Image>();
        controlsTabImage = controlsTabButton.GetComponent<Image>();
    }

    void Start()
    {
        ShowSettings();
    }

    public void ShowSettings()
    {
        SwitchPanel(
            showPanel: settingsPanel,
            showGroup: settingsGroup,
            hidePanel: controlsPanel,
            hideGroup: controlsGroup
        );

        settingsTabImage.color = activeColor;
        controlsTabImage.color = inactiveColor;
    }

    public void ShowControls()
    {
        SwitchPanel(
            showPanel: controlsPanel,
            showGroup: controlsGroup,
            hidePanel: settingsPanel,
            hideGroup: settingsGroup
        );

        settingsTabImage.color = inactiveColor;
        controlsTabImage.color = activeColor;
    }

    void SwitchPanel(
        GameObject showPanel,
        CanvasGroup showGroup,
        GameObject hidePanel,
        CanvasGroup hideGroup
    )
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTransition(showPanel, showGroup, hidePanel, hideGroup));
    }

    IEnumerator FadeTransition(
        GameObject showPanel,
        CanvasGroup showGroup,
        GameObject hidePanel,
        CanvasGroup hideGroup
    )
    {
        showPanel.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = t / fadeDuration;

            showGroup.alpha = Mathf.Lerp(0f, 1f, a);
            hideGroup.alpha = Mathf.Lerp(1f, 0f, a);

            yield return null;
        }

        showGroup.alpha = 1f;
        hideGroup.alpha = 0f;

        hidePanel.SetActive(false);
    }
}
