using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMainMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenuTest";
    public void GoBack()
    {
        if (SceneFadeManager.I != null)
            SceneFadeManager.I.FadeToScene(mainMenuScene);
        else
            SceneManager.LoadScene(mainMenuScene);
    }
}