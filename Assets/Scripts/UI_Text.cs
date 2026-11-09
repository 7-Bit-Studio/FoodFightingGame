

//  System
using System;
using System.IO;

// Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Custom Datatypes
using Assets.Globals;
using TMPro;

public class UIText : MonoBehaviour
{
    /// <summary>
    /// Number of frames to average over
    /// </summary>
    [SerializeField] private uint averageDuration = 5;
    [SerializeField] private Text FPSTextElement;
    [SerializeField] private Text SpeedElement;
    [SerializeField] private Text PositionElement;
    [SerializeField] private bool debugLogging = false;
    //[SerializeField] private TextMeshProUGUI WASDPrompt;
    //[SerializeField] private GameObject promptBackground;
    private Transform TopLeftCorner;
    private Functions funcs = new();
    private FunctionsInstance funcInst = new();
    private PlayerController movement;
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
    private VarTypes.Data data;
    private int screenWidth;
    private int screenHeight;
    private bool showDebugInfo;
    //private bool showWASDPrompt;
    //private bool wPressed;
    //private bool aPressed;
    //private bool sPressed;
    //private bool dPressed;

    void Loginit_()
    {
        docPath += "\\FPS-Logs";
        docName = $"FPS-{DateTime.Now:dd-MM-yyyy_HH.mm.ss.fff}.csv";
        File.AppendAllText(Path.Combine(docPath, docName), $"Time,\tFPS\n");
    }
    void InitializeVars()
    {
        if (debugLogging) Loginit_();
        movement = GetComponent<PlayerController>();
        fpsLow = float.PositiveInfinity;
        data = movement.GetData();
        playerVel = data.velocity;
        
        prevPlayerVel1 = new Vector3[averageDuration];
        prevPlayerVel2 = new Vector3[averageDuration + 1];
        showDebugInfo = false;
        //showWASDPrompt = true;
    }

    void ScreenInit()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        TopLeftCorner = GetComponent<Transform>();
        float margin = 0.05f;
        // -screenWidth/2 and screenHeight/2 to align with the top-left corner
        // 0.95 for a 5% padding margin
        TopLeftCorner.localPosition = new Vector3(-(screenWidth / 2) * (1 - margin), ((1 - margin) * screenHeight/2) + screenHeight/2);
        //abovePlayer.localPosition = new Vector3(-(screenWidth / 2) * (1 - margin), (1 - margin) * screenHeight / 2);
    }

    void Start()
    {
        ScreenInit();
        InitializeVars();
    }

    // Update is called once per frame
    public void Update()
    {
        HandleInputs();

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
        //if (showWASDPrompt)
        //{
            
        //}

        data = movement.GetData();
        playerVel = data.velocity;
        playerPos = data.position;
        deltaTime = Time.deltaTime;

        prevPlayerVel1 = prevPlayerVel2[0..(int)averageDuration];
        prevPlayerVel2[0] = playerVel;

        for(int i = 0; i < averageDuration; i++)
        {
            prevPlayerVel2[i + 1] = prevPlayerVel1[i];
        }

        smoothedPlayerVel = funcInst.AverageOverFrames(prevPlayerVel2, (int)averageDuration);
        smoothedPlayerVel = funcInst.Round(smoothedPlayerVel, 2);
        
        roundedFPS = funcInst.Round(currFPS, 1);
        playerPos = funcInst.Round(playerPos, 1) * 10;


        FPS();
        if (debugLogging) FPSFileWrite();
        SetText(FPSTextElement,$"{roundedFPS}fps");
        SetText(SpeedElement, $"{smoothedPlayerVel.x}, {smoothedPlayerVel.y}");
        SetText(PositionElement, $"X: {playerPos.x} Y: {playerPos.y}\nT: {funcInst.Round(Time.timeAsDouble,2)} sec");
	}

    void HandleInputs()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            ScreenInit();
        }

        if (Keyboard.current.altKey.wasPressedThisFrame)
        {
            showDebugInfo = !showDebugInfo;
        }

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
        File.AppendAllText(Path.Combine(docPath, docName), $"{Time.time},\t{currFPS}\n");
    }
}