//  System
using System;
using System.IO;

// Unity
using Unity;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.UI;

public class UIText : MonoBehaviour
{
    [SerializeField] private Text textElement;
    // Start is called before the first frame update
    private float fpsLow;
    private float currFPS;
    private string docPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private string docName;
    void Start()
    {
        docPath += "\\FPS-Logs";
        docName = $"FPS-{DateTime.Now:dd-MM-yyyy_HH.mm.ss.fff}.csv";
        File.AppendAllText(Path.Combine(docPath, docName), $"FPS,\tTime\n");
        fpsLow = float.PositiveInfinity;
    }

	// Update is called once per frame
	void Update()
	{
        FPS();
        FPSFileWrite();
        textElement.text = $"{currFPS}fps\n{fpsLow}fps min";
	}

    void FPS()
    {
        float dt = Time.deltaTime;
        currFPS = 1 / dt;

        fpsLow = Mathf.Min(currFPS, fpsLow);
    }

    void FPSFileWrite()
    {
        File.AppendAllText(Path.Combine(docPath, docName), $"{currFPS},\t{Time.time}\n");
    }
}