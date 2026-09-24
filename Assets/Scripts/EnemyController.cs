// Unity
using System;
using Unity;
using Unity.U2D;
using Unity.Mathematics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.InputSystem;

// Globals
using Assets.Globals;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public struct Data
    {
        public Vector3 enemyVel;
        public Transform enemyPos;
    }

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float enemyAcc = 5f;
    [SerializeField] private float frictionCoefficient = 1.01f;
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private Texture2D texture2D;

    // Player values
    private Transform enemyPos;
    private Vector3 enemyVel;

    // PlayerData Values
    private VarTypes.Direction facing;
    private Data data;
    public Data GetData() => data;

    private Sprite[] spriteArr;
    private Sprite currentSprite;
    private int currentSpriteIndex;

    // DeltaTime
    private float deltaTime;
    private float timeAtLastSpriteUpdate;

    void Awake()
    {

        enemyPos = GetComponent<Transform>();

        data.enemyVel = enemyVel;
        data.enemyPos = enemyPos;
        timeAtLastSpriteUpdate = 0f;
        currentSpriteIndex = 0;
    }

    private void Start()
    {
        spriteArr[0] = texture2D.GetComponent<Sprite>();
        Debug.Log(spriteArr.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeAtLastSpriteUpdate - Time.time > 0.25f)
        {
            currentSprite = spriteArr[currentSpriteIndex];
            timeAtLastSpriteUpdate = Time.time;
            GetComponent<SpriteRenderer>().sprite = currentSprite;
            currentSpriteIndex += 1;
            currentSpriteIndex %= spriteArr.Length;
        }
    }
}
