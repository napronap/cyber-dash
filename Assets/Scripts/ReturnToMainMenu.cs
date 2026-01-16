using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public void YesGoToMainMenu()
    {
       GameFlowManager.I.YesReturnToMainMenu();
    }

    public void NoBackToPause()
    {
        GameFlowManager.I.NoReturnToPause();
    }
}
