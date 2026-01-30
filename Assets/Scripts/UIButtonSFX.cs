using UnityEngine;
using UnityEngine.EventSystems;
public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Auto: finds a single UI AudioSource by tag 'UIAudio' (recommended)")]
    public AudioSource uiSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;
    void Awake()
    {
        // If not assigned manually, auto-find by tag
        if (uiSource == null)
        {
            var go = GameObject.FindGameObjectWithTag("UIAudio");
            if (go != null) uiSource = go.GetComponent<AudioSource>();
        }
        // Fallback: find any AudioSource in scene named "UIAudio"
        if (uiSource == null)
        {
            var go = GameObject.Find("UIAudio");
            if (go != null) uiSource = go.GetComponent<AudioSource>();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiSource != null && hoverClip != null)
            uiSource.PlayOneShot(hoverClip);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiSource != null && clickClip != null)
            uiSource.PlayOneShot(clickClip);
    }
}