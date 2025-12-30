using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    bool paused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0f;

        SceneFadeManager.I.FadeOut();
        pauseMenuCanvas.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        paused = false;

        pauseMenuCanvas.SetActive(false);
        SceneFadeManager.I.FadeIn();

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
