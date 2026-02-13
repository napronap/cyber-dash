using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{
    public string pauseSceneName = "Pause";
    public string optionsSceneName = "Options";
    bool isPaused;
    //void Update()
    //{
        //if (Input.GetKeyDown(KeyCode.Escape))
       // {
            //if (isPaused) Resume();
            //else Pause();
        //}
    //}
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadSceneAsync(pauseSceneName, LoadSceneMode.Additive);
    }
    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.UnloadSceneAsync(pauseSceneName);
    }
    public void GoToOptions()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (SceneManager.GetSceneByName(pauseSceneName).isLoaded)
            SceneManager.UnloadSceneAsync(pauseSceneName);
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene(optionsSceneName);
        else
            SceneManager.LoadScene(optionsSceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (SceneManager.GetSceneByName("Pause").isLoaded)
            SceneManager.UnloadSceneAsync("Pause");

        string currentScene = SceneManager.GetActiveScene().name;

        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene(currentScene);
        else
            SceneManager.LoadScene(currentScene);
    }
}