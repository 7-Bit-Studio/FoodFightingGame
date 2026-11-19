using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        Debug.Log("Starting!!");
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting!!");
        Application.Quit();
    }

    public void Options()
    {
        Debug.Log("Opening Options");
    }
}
