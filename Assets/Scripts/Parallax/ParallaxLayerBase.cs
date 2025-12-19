using UnityEngine;

public abstract class ParallaxLayerBase : MonoBehaviour
{
    public float speed = 2f;
    public bool isActive = true;
    void Start()
    {

    }

    void Update()
    {
        if (!isActive) return;

        Tick(Time.deltaTime);
    }

    protected abstract void Tick(float dt);
}
