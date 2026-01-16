using NUnit.Framework.Internal;
using UnityEngine;

public class PauseUIRoot : MonoBehaviour
{
    public static PauseUIRoot I;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        I = this;
    }

    // Update is called once per frame
    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
