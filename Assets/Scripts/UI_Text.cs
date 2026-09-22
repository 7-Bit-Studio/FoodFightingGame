

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
using UnityEngine.InputSystem;

// Custom Datatypes
using Assets.GlobalFuncs;
using Unity.VisualScripting;
using System.Linq;

public class UIText : MonoBehaviour
{
    /// <summary>
    /// number of frames to average over
    /// </summary>
    [SerializeField] private uint averageDuration = 5;
    [SerializeField] private Text FPSTextElement;
    [SerializeField] private Text SpeedElement;
    private Functions functions;
    private Movement movement;
    private float fpsLow;
    private float currFPS;
    private string docPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private string docName;
    private float deltaTime;
    private Vector3 playerVel;
    private Vector3 smoothedPlayerVel;
    private Vector3[] prevPlayerVel1;
    private Vector3[] prevPlayerVel2;
    private Movement.Data data;
    void Loginit_()
    {
        docPath += "\\FPS-Logs";
        docName = $"FPS-{DateTime.Now:dd-MM-yyyy_HH.mm.ss.fff}.csv";
        File.AppendAllText(Path.Combine(docPath, docName), $"FPS,\tTime\n");
    }
    void InitializeVars()
    {
        Loginit_();
        fpsLow = float.PositiveInfinity;
        functions = new Functions();
        movement = GetComponent<Movement>();
        data = movement.GetData();
        playerVel = data.playerVel;
        prevPlayerVel1 = new Vector3[averageDuration];
        prevPlayerVel2 = new Vector3[averageDuration + 1];
    }

    void Start()
    {
        InitializeVars();
    }

    // Update is called once per frame
    public void Update()
    {
        data = movement.GetData();
        playerVel = data.playerVel;
        deltaTime = Time.deltaTime;

        prevPlayerVel1 = prevPlayerVel2[0..(int)averageDuration];
        prevPlayerVel2[0] = playerVel;
        for(int i = 0; i < averageDuration; i++)
        {
            prevPlayerVel2[i + 1] = prevPlayerVel1[i];
        }

        smoothedPlayerVel = functions.AverageOverFrames(prevPlayerVel2);
        smoothedPlayerVel = functions.Round(smoothedPlayerVel, 2);

        FPS();
        FPSFileWrite();
        SetText(FPSTextElement,$"{currFPS}fps\n{fpsLow}fps min");
        SetText(SpeedElement, $"{smoothedPlayerVel.x}, {smoothedPlayerVel.y}");
        Debug.Log($"{smoothedPlayerVel.x}, {smoothedPlayerVel.y}");
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