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
        Tick(Time.deltaTime);
    }

    public virtual void toggleActive()
    {
        isActive = !isActive;
    }

    protected abstract void Tick(float dt);
}
