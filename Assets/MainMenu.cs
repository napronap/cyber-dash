using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("START");
        //SceneManager.LoadScene(1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {
        SceneManager.LoadScene("Options"); // <-- scene name here
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        Application.Quit();
    }
}
