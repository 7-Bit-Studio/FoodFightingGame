

//  System
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

// Unity
using Unity;
using Unity.VectorGraphics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Custom Datatypes
using Assets.Globals;

public class UIText : MonoBehaviour
{
    /// <summary>
    /// Number of frames to average over
    /// </summary>
    [SerializeField] private uint averageDuration = 5;
    [SerializeField] private Text FPSTextElement;
    [SerializeField] private Text SpeedElement;
    [SerializeField] private Text PositionElement;
    private Transform TopLeftCorner;
    private Functions functions;
    private Movement movement;
    private float fpsLow;
    private float currFPS;
    private float roundedFPS;
    private string docPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private string docName;
    private float deltaTime;
    private Vector3 playerVel;
    private Vector3 playerPos;
    private Vector3 smoothedPlayerVel;
    private Vector3[] prevPlayerVel1;
    private Vector3[] prevPlayerVel2;
    private Movement.Data data;
    private int screenWidth;
    private int screenHeight;
    private bool showDebugInfo;

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
        showDebugInfo = false;
    }

    void ScreenInit()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        TopLeftCorner = GetComponent<Transform>().Find("UI").GetChild(0).GetChild(0).GetChild(0);
        float margin = 0.05f;
        // -screenWidth/2 and screenHeight/2 to align with the top-left corner
        // 0.95 for a 5% padding margin
        TopLeftCorner.localPosition = new Vector3(-(screenWidth / 2) * (1 - margin), (1 - margin) * screenHeight / 2);
    }

    void Start()
    {
        ScreenInit();
        InitializeVars();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            ScreenInit();
        }

        if (Keyboard.current.altKey.wasReleasedThisFrame)
        {
            showDebugInfo = !showDebugInfo;
        }

        if (!showDebugInfo)
        {
            PositionElement.gameObject.SetActive(false);
            FPSTextElement.gameObject.SetActive(false);
            SpeedElement.gameObject.SetActive(false);
        }
        else if (showDebugInfo)
        {
            PositionElement.gameObject.SetActive(true);
            FPSTextElement.gameObject.SetActive(true);
            SpeedElement.gameObject.SetActive(true);
        }
        data = movement.GetData();
        playerVel = data.playerVel;
        playerPos = data.playerPos.position;
        deltaTime = Time.deltaTime;

        prevPlayerVel1 = prevPlayerVel2[0..(int)averageDuration];
        prevPlayerVel2[0] = playerVel;

        for(int i = 0; i < averageDuration; i++)
        {
            prevPlayerVel2[i + 1] = prevPlayerVel1[i];
        }

        smoothedPlayerVel = functions.AverageOverFrames(prevPlayerVel2, (int)averageDuration);
        smoothedPlayerVel = functions.Round(smoothedPlayerVel, 2);
        
        roundedFPS = functions.Round(currFPS, 1);
        playerPos = functions.Round(playerPos, 1) * 10;

        FPS();
        FPSFileWrite();
        SetText(FPSTextElement,$"{roundedFPS}fps");
        SetText(SpeedElement, $"{smoothedPlayerVel.x}, {smoothedPlayerVel.y}");
        SetText(PositionElement, $"X: {playerPos.x} Y: {playerPos.y}\nT: {functions.Round(Time.timeAsDouble,2)} sec");
	}

    void SetText(Text textElement, string text)
    {
        textElement.text = text;
    }

    void FPS()
    {
        currFPS = 1 / deltaTime;

        fpsLow = Mathf.Min(currFPS, fpsLow);
    }

    void FPSFileWrite()
    {
        File.AppendAllText(Path.Combine(docPath, docName), $"{currFPS},\t{Time.time}\n");
    }
}