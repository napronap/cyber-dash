using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    private static HitStop instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Do(float duration)
    {
        if (instance == null) return;
        instance.StartCoroutine(instance.HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        float originalScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalScale;
    }
}
