using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoToReturntoMain()
    {
        SceneManager.LoadScene("ReturnToMain");
    }
}
