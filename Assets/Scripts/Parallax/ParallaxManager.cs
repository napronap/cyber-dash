using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public ParallaxLayerBase[] layers;

    public void SetScrolling(bool active)
    {
        foreach (var layer in layers)
        {
            layer.isActive = active;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space");
            SetScrolling(false);
        }
    }
}
