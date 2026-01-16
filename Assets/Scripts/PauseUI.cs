using UnityEngine;
public class PauseUI : MonoBehaviour
{
    public void Resume()
    {
        GameFlowManager.I.ClosePause();
    }
    public void Quit()
    {
        GameFlowManager.I.OpenReturnToMain();
    }
    public void GoToOptions()
    {
        // close pause + unpause before going options
        GameFlowManager.I.ClosePause();
        // then go options (via SceneFadeManager if you want)
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene("Options");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Options");
    }
    public void RestartGame()
    {
        // unpause + close pause, then reload current scene
        GameFlowManager.I.ClosePause();
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene(scene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}