using UnityEngine;
using UnityEngine.SceneManagement;
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager I;
    public string pauseScene = "Pause";
    public string returnToMainScene = "ReturnToMain";
    public string mainMenuScene = "MainMenuTest";
    bool pauseLoaded;
    bool confirmLoaded;
    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;
        {
            // If confirm open, ESC just closes confirm
            if (confirmLoaded)
            {
                NoReturnToPause();
                return;
            }
            if (!pauseLoaded) 
                OpenPause();
            else ClosePause();
        }
    }
    public void OpenPause()
    {
        if (pauseLoaded) return;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadSceneAsync(pauseScene, LoadSceneMode.Additive);
        pauseLoaded = true;
    }
    public void ClosePause()
    {
        if (!pauseLoaded) return;
        // Close confirm first if open
        if (confirmLoaded)
            SceneManager.UnloadSceneAsync(returnToMainScene);
        confirmLoaded = false;
        SceneManager.UnloadSceneAsync(pauseScene);
        pauseLoaded = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Called from Pause UI Quit button
    public void OpenReturnToMain()
    {
        if (confirmLoaded) return;
        SetPauseUIVisible(false);

        SceneManager.LoadSceneAsync(returnToMainScene, LoadSceneMode.Additive);
        confirmLoaded = true;
    }
    // NO button: go back to Pause overlay (game still frozen)
    public void NoReturnToPause()
    {
        if (!confirmLoaded) return;

        SceneManager.UnloadSceneAsync(returnToMainScene);
        confirmLoaded = false;

        SetPauseUIVisible(true);
        // Stay paused (do NOT change timeScale)
        Time.timeScale = 0f;
    }
    // YES button: go to main menu
    public void YesReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (confirmLoaded) SceneManager.UnloadSceneAsync(returnToMainScene);
        if (pauseLoaded) SceneManager.UnloadSceneAsync(pauseScene);
        confirmLoaded = false;
        pauseLoaded = false;
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene(mainMenuScene);
        else
            SceneManager.LoadScene(mainMenuScene);
    }

    void SetPauseUIVisible(bool visible)
    {
        var ui = FindFirstObjectByType<PauseUIRoot>();
        if (ui != null) ui.SetVisible(visible);
    }
}