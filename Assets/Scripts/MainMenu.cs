using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene("Scene_Game");
        else
            SceneManager.LoadScene("Scene_Game");
    }

    public void Options()
    {
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene("Options");
        else
            SceneManager.LoadScene("Options"); // <-- scene name here
    }

    public void Credits()
    {
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene("Credits");
        else
            SceneManager.LoadScene("Credits"); // <-- scene name here
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene("Quit");
        else
            SceneManager.LoadScene("Quit"); // <-- scene name here
    }
}
