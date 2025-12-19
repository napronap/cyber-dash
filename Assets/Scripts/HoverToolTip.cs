using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform tipRoot;   // the HoverTip panel
    public TMP_Text tipText;        // HoverTipText
    [TextArea] public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tipText) tipText.text = message;
        if (tipRoot) tipRoot.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tipRoot) tipRoot.gameObject.SetActive(false);
    }
}
