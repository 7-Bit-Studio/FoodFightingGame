using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] private int fps = 60;
    private int targetFPS;

    void initFPS()
    {
        targetFPS = fps;
        Application.targetFrameRate = targetFPS;
    }
    // Start is called before the first frame update
    void Start()
	{
        QualitySettings.vSyncCount = 1;
    }

	// Update is called once per frame
	void Update()
	{
        if (fps != targetFPS)
        {
            initFPS();
        }
    }
}