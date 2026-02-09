using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartUI : MonoBehaviour
{
    private Image image;
    private Vector3 originalScale;
    private Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = image.color;
    }

    public void PlayLoseAnim()
    {
        StopAllCoroutines();
        StartCoroutine(LoseRoutine());
    }

    private IEnumerator LoseRoutine()
    {
        yield return null;
        // tried making an animation but it sucks oh well
        // pop
        // transform.localScale = originalScale * 1.3f;
        // image.color = Color.red;

        // yield return new WaitForSeconds(0.1f);

        // // shrink + fade
        // float t = 0f;
        // float dur = 0.15f;

        // while (t < dur)
        // {
        //     float k = t / dur;
        //     transform.localScale = Vector3.Lerp(originalScale * 1.3f, originalScale * 0.5f, k);
        //     image.color = Color.Lerp(Color.red, new Color(1, 1, 1, 0), k);
        //     t += Time.deltaTime;
        //     yield return null;
        // }

        // transform.localScale = originalScale;
        // image.color = originalColor;
        // image.enabled = false;
    }
}
