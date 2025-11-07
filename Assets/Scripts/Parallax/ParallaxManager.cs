using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public ParallaxLayer[] layers;

    public void SetScrolling(bool active)
    {
        foreach (var layer in layers)
        {
            layer.isActive = active;
        }
    }
}
