using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Score += amount;
    }

    public void ResetScore()
    {
        Score = 0;
    }
}
