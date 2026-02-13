using UnityEngine;

public class PauseBridge : MonoBehaviour
{
    PauseManager pm;
    void Start()
    {
        pm = FindFirstObjectByType<PauseManager>();
        if (pm == null)
            Debug.LogError("PauseManager not found! Make sure it exists in the Game scene.");
    }
    public void Resume()
    {
        pm.Resume();
    }
    public void GoToOptions()
    {
        pm.GoToOptions();
    }
    public void RestartGame()
    {
        pm.RestartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
