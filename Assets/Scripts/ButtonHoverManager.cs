using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject deetsPanel;
    public Sprite panelSprite;
    public string text = "Deets";

    public float fadeDuration = 0.25f;

    private CanvasGroup canvasGroup;
    private TMPro.TextMeshProUGUI tmpText;
    private Image panelImage;

    private void Awake()
    {
        canvasGroup = deetsPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = deetsPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        deetsPanel.SetActive(false);

        tmpText = deetsPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        panelImage = deetsPanel.GetComponentInChildren<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        deetsPanel.SetActive(true);

        // Optional: update sprite and text
        if (panelImage != null) panelImage.sprite = panelSprite;
        if (tmpText != null) tmpText.text = text;

        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup, canvasGroup.alpha, 1f, fadeDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup, canvasGroup.alpha, 0f, fadeDuration, true));
    }
    private IEnumerator Fade(CanvasGroup cg, float start, float end, float duration, bool deactivateAfter = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
        if (deactivateAfter && end == 0f)
            deetsPanel.SetActive(false);
    }
}
