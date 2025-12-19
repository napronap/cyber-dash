using UnityEngine;

public class ReturnToMainMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
       SceneFadeManager.I.FadeToScene("MainMenuTest");
    }
}
