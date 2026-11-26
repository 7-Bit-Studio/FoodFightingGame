using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    //private AudioSource[] aSources;
    [SerializeField] private GameObject sound;

    [SerializeField] private PlayerController player;
    private UIText uiMenu;
    private AudioSource audio;

    private AudioSource biteSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //aSources = GetComponents<AudioSource>();
        audio = sound.GetComponent<AudioSource>();
        uiMenu = player.GetComponent<UIText>();

    }
    public void StartGame()
    {
        Debug.Log("Starting!!");
        audio.Play();
        DontDestroyOnLoad(sound);

        SceneManager.LoadScene(2);
    }

    public void ResumeGame()
    {
        uiMenu.showPauseMenu = false;
    }



    public void QuitGame()
    {
        Debug.Log("Quitting!!");
        Application.Quit();
    }

    public void Options()
    {
        Debug.Log("Opening Options");
        SceneManager.LoadScene(1);

    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
