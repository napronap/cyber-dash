using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
       SceneFadeManager.I.FadeToScene("MainMenuTest");
    }

    public void NoBackToPause()
    {
        SceneManager.LoadScene("Pause");
    }
}
