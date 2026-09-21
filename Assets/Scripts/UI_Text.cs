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
    private string docPath = Path.GetFullPath(Environment.CurrentDirectory);
    private string docName;
    void Start()
    {
        docPath += "\\Assets";
        docName = $"FPS-{DateTime.Now:dd-MM-yyyy_HH.mm.ss.fff}.csv";
        Debug.Log(docPath);
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
        Debug.Log(docPath);
        File.AppendAllText(Path.Combine(docPath, docName), $"{currFPS},\t{Time.time}\n");
    }
}