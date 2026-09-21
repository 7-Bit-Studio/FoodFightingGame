using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIText : MonoBehaviour
{
    [SerializeField] private Text textElement;
    // Start is called before the first frame update
    private float fpsLow;
    private float currFPS;
    void Start()
    {
        fpsLow = float.PositiveInfinity;
    }

	// Update is called once per frame
	void Update()
	{
        FPS();
        textElement.text = $"{currFPS}fps\n{fpsLow}fps min";
	}

    void FPS()
    {
        float dt = Time.deltaTime;
        currFPS = 1 / dt;

        fpsLow = Mathf.Min(currFPS, fpsLow);
    }
}