using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public ParallaxLayerBase[] layers;
    public static ParallaxManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetScrolling(bool active)
    {
        if (layers == null)
        {
            return;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null)
            {
                continue;
            }

            layers[i].isActive = active;
        }
    }

    void Update()
    {
    }
}
