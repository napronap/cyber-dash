using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public ParallaxLayerBase[] layers;

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
