

//  System
using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;

// Unity
using Unity;

// Unity Engine
using UnityEngine;
using UnityEngine.UI;

// Custom Datatypes
using Assets.Scripts;
using Assets.GlobalFuncs;

public class UIText : MonoBehaviour
{

    [SerializeField] private Text FPSTextElement;
    [SerializeField] private Text SpeedElement;

    private PlayerData movement;
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
        movement = new PlayerData();
        functions = new Functions();
        fpsLow = float.PositiveInfinity;
    }

    void Awake()
    {
        InitializeVars();
    }

    // Update is called once per frame
    void Update()
    {
        playerVel = movement.playerVel;
        deltaTime = Time.deltaTime;
        FPS();
        FPSFileWrite();
        SetText(FPSTextElement,$"{currFPS}fps\n{fpsLow}fps min");
        SetText(SpeedElement, $"{playerVel.x}, {playerVel.y}");
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