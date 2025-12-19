using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("StartGameTest");
    }

    public void Options()
    {
        SceneManager.LoadScene("Options"); // <-- scene name here
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits"); // <-- scene name here
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        SceneManager.LoadScene("Quit"); // <-- scene name here
    }
}
