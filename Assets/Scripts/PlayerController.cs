// Unity
using System;
using Unity;
using Unity.Mathematics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.InputSystem;

// Globals
using Assets.Globals;
    
/// <summary>
    /// Data about the player
    /// </summary>
    [RequireComponent(typeof(Transform))]
    
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float playerAcc = 5f;
    [SerializeField] private float frictionCoefficient = 1.01f;
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private int health = 100;
    [SerializeField] private int defense = 0;
    [SerializeField] private float attackRange = 2.5f;
    
    // PlayerController values
    private Transform playerPos;
    private Vector3 playerVel;
    
    // PlayerData Values
    private VarTypes.Direction facing;
    private Vector2 moveInputVector2;
    private InputAction moveAction;
    private Vector3 moveInput;
    private VarTypes.Data data;
    public VarTypes.Data GetData() => data;

    // DeltaTime
    private float deltaTime;
    private GameObject[] livingEnemies;
    private float[] enemyDistances;
    private LineRenderer lineRenderer;

    void Awake()
    {
        // Preliminary checks
        if (moveActionReference == null)
        {
            Debug.LogError("no InputActionReference");
            return;
        }

        lineRenderer = GetComponent<LineRenderer>();

        if(lineRenderer == null)
        {
            lineRenderer = new GameObject().AddComponent<LineRenderer>();
        }
    
        // Initializing values
        moveInputVector2 = Vector2.zero;
        moveInput = Vector3.zero;
        playerVel = Vector3.zero;
    
        playerPos = GetComponent<Transform>();
    
        data.velocity = playerVel;
        data.position = playerPos;
        data.health = health;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        transform.Find("Main Camera").position = new Vector3(0, 0, -10);
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Setting current-frame constants
        deltaTime = Time.deltaTime;
        moveAction = moveActionReference.action;


        HandleInputs();
    
        // Move Input Vector
    
        moveInputVector2 = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        moveInput = (Vector3)moveInputVector2;



        UpdateMovement();

        UpdatePlayerFacingDirection();
        livingEnemies = GetLivingEnemies();
        DrawCircle(attackRange, transform.position, 0.01f);

    }

    GameObject[] GetLivingEnemies()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>();
        GameObject[] livingEnemies = new GameObject[enemies.Length];
        enemyDistances = new float[enemies.Length];

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemy = enemies[i];
            livingEnemies[i] = enemy.gameObject;
            enemyDistances[i] = Vector3.Distance(transform.position, enemy.transform.position);
        }

        return livingEnemies;
    }
    
    void Attack(GameObject enemy, int damage)
    {
        enemy.GetComponent<EnemyController>().OnHit(damage);
    }

    void HandleInputs()
    {
        if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.qKey.wasReleasedThisFrame)
        {
            string debugImageName = "Hide-the-pain-Harold-large-meme-8_0";
            transform.Find(debugImageName).gameObject.SetActive(!transform.Find(debugImageName).gameObject.activeSelf);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            float minDistance = Mathf.Min(enemyDistances);

            if(minDistance > attackRange) return;

            int indexOfMinDistance = Array.IndexOf(enemyDistances, minDistance);
            Attack(livingEnemies[indexOfMinDistance], 10);
        }
    }

    void UpdateMovement()
    {
        if (useAcceleration)
        {
            AccelerationMovement();
        }
        else
        {
            VelocityMovement();
        }
    
        UpdatePosition();
        data.velocity = playerVel;
        data.position = playerPos;

        if(playerVel.x > 0)
        {
            facing = VarTypes.Direction.Left;
        }
        else
        {
            facing = VarTypes.Direction.Right;
        }

        data.direction = facing;
    }

    void UpdatePlayerFacingDirection()
    {
        if(facing == VarTypes.Direction.Left)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y,transform.localScale.z);
            transform.Find("UI").localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            transform.Find("UI").localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }
    
    // Updateing the player's velocity
    void VelocityMovement()
    {
        // PlayerData in metres per second
    
        playerVel = maxSpeed * moveInput;
    }
    
    // Updating the player's acceleration
    void AccelerationMovement()
    {
        Vector3 currentAcc = playerAcc * moveInput;
    
        playerVel += deltaTime * currentAcc;
    
        if (playerVel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            playerVel = playerVel.normalized * maxSpeed;
        }

        if (moveInput.sqrMagnitude < 0.01)
        {
            playerVel = Friction(playerVel, frictionCoefficient);
        }
        if(Mathf.Sign(moveInput.x) != Mathf.Sign(playerVel.x))
        {
            if(Mathf.Sign(moveInput.y) != Mathf.Sign(playerVel.y))
            {
                playerVel = Friction(playerVel, frictionCoefficient);
            }
        }
    }
    void UpdatePosition()
    {
        playerPos.position += deltaTime * playerVel;
        /*
         * Set the player's position, then rotation
         * Can be simplified to the line below
         * 
         * transform.SetPositionAndRotation(playerPos.position, playerPos.rotation);
         */

        transform.position = playerPos.position;
        transform.rotation = playerPos.rotation;
    }

    Vector3 Friction(Vector3 vel, float fricCoef)
    {
        return vel / fricCoef;
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                       new Vector4(0, 0, 1, 0),
                                       new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

        }
    }

    public void DrawCircle(float radius, Vector3 centerPos, float strokeWidth)
    {
        DrawPolygon(100, radius, centerPos, strokeWidth, strokeWidth);
    }
}
