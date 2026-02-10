using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (scoreText != null && ScoreManager.Instance != null)
        {
            scoreText.text = "" + ScoreManager.Instance.Score;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuTest");
    }
}
