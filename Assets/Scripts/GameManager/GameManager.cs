using UnityEngine;

public enum GameState { Running, Paused, End }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; }

    [Header("Performance")]
    public int targetFPS = 60;

    [Header("Gameplay Speed")]
    public float baseScrollSpeed = 3f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = targetFPS;
    }
    void Update()
    {


    }

    public void SetState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Running:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.End:
                FindFirstObjectByType<ParallaxManager>().SetScrolling(false);
                break;
        }
    }
}
