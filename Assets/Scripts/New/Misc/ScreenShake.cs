using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    private static ScreenShake instance;
    private Vector3 originalPos;

    private void Awake()
    {
        instance = this;
        originalPos = transform.localPosition;
    }

    public static void Shake(float duration, float strength)
    {
        if (instance == null) return;
        instance.StartCoroutine(instance.ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float time = 0f;

        while (time < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
