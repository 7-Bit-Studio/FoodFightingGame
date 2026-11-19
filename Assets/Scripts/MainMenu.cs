using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //private AudioSource[] aSources;
    [SerializeField] private GameObject sound;
    private AudioSource audio;

    private AudioSource biteSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //aSources = GetComponents<AudioSource>();
        audio = sound.GetComponent<AudioSource>();

    }
    public void StartGame()
    {
        Debug.Log("Starting!!");
        audio.Play();
        DontDestroyOnLoad(sound);

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
