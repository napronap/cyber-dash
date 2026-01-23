using UnityEngine;

public class MusicPersist : MonoBehaviour
{
    private static MusicPersist instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
