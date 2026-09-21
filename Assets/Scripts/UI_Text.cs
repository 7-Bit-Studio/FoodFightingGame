

//  System
using System;
using System.IO;

// Unity
using Unity;

// Unity Engine
using UnityEngine;
using UnityEngine.UI;

// Global Functions
using Assets.GlobalFuncs;

[RequireComponent(typeof(Movement))]
public class UIText : MonoBehaviour
{
    [SerializeField] private Text FPSTextElement;
    [SerializeField] private Text SpeedElement;

    private Movement movement;
    // Start is called before the first frame update
    private Functions functions;
    private float fpsLow;
    private float currFPS;
    private string docPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private string docName;
    private float deltaTime;
    private Vector3 playerVel;
    void Loginit_()
    {
        docPath += "\\FPS-Logs";
        docName = $"FPS-{DateTime.Now:dd-MM-yyyy_HH.mm.ss.fff}.csv";
        File.AppendAllText(Path.Combine(docPath, docName), $"FPS,\tTime\n");
    }
    void InitializeVars()
    {
        Loginit_();
        functions = new Functions();
        movement = new Movement();
        playerVel = movement.PlayerVel;
        fpsLow = float.PositiveInfinity;
    }

    void Awake()
    {
        InitializeVars();
    }

    // Update is called once per frame
    void Update()
    {
        playerVel = functions.Round(movement.PlayerVel, 2);
        deltaTime = Time.deltaTime;
        FPS();
        FPSFileWrite();
        SetText(FPSTextElement,$"{currFPS}fps\n{fpsLow}fps min");
        SetText(SpeedElement, $"{playerVel.x}, {playerVel.y}, {playerVel.z}");
	}

    void SetText(Text textElement, string text)
    {
        textElement.text = text;
    }

    void FPS()
    {
        currFPS = 1 / deltaTime;

        currFPS = functions.Round(currFPS, 2);

        fpsLow = Mathf.Min(currFPS, fpsLow);
    }

    void FPSFileWrite()
    {
        File.AppendAllText(Path.Combine(docPath, docName), $"{currFPS},\t{Time.time}\n");
    }

    
}