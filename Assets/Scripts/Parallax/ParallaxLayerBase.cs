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

    public virtual void toggleActive(bool? nextState = null)
    {
        if (nextState != null)
        {
            isActive = (bool)nextState;
            return;
        }

        isActive = !isActive;
    }

    protected abstract void Tick(float dt);
}
