// System
using System;
using System.Linq;
using System.Collections.Generic;
// Custom funcs and variable types
using Assets.Globals;
// Unity
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Unity.VisualScripting;


[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// Data about the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private Texture2D playerTexture;
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float playerAcc = 5f;
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private GameObject[] MeleeAttacks;
    [SerializeField] private GameObject[] RangedAttacks;
    [SerializeField] private bool cheatMode = false;
    [SerializeField] private float attackLenience = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    
    // PlayerData Values
    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private int defense = 0;
    [SerializeField] private bool canDie = true;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashSpeed = 2f;
    private InputAction moveAction;
    public VarTypes.Data GetData() => data;
    private VarTypes.Direction facing;
    private VarTypes.Data data;
    private Vector2 moveInputVector2;
    private Vector3 moveInput;
    private int baseMult = 1;

    // PlayerController values
    private Transform playerTransform;
    private Vector3 playerPos;
    private Vector3 playerVel;

    // DeltaTime
    private float deltaTime;
    private VarTypes.EnemyData[] nearbyLivingEnemies;
    private LineRenderer lineRenderer;
    private float timeAtLastAttack;
    private readonly Functions funcs = new();
    private readonly FunctionsInstance funcInst = new();
    private int animationDirectionMultiplier = 1;

    // Player Animation
    private Sprite[] playerSprites;
    private Sprite currentSprite;
    private int currentSpriteIndexAdd;
    private float timeAtLastSpriteUpdate;
    private float timeAtLastHit;
    private float timeAtLastDash;
    private int attackIndex = -1;
    public VarTypes.Item[] inventory;
    [SerializeField] private GameObject playerMeleeAttackPrefab;
    [SerializeField] private GameObject playerRangedAttackPrefab;
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
            lineRenderer = new GameObject("linerenderer").AddComponent<LineRenderer>();
        }

        if(MeleeAttacks == null || MeleeAttacks.Length == 0)
        {
            MeleeAttacks = new GameObject[] { Instantiate(playerMeleeAttackPrefab) };
        }
        if(RangedAttacks == null || RangedAttacks.Length == 0)
        {
            RangedAttacks = new GameObject[] { Instantiate(playerRangedAttackPrefab) };
        }

        if (GameOverScreen == null)
        {
            Debug.LogWarning("GameOverScreen is not assigned in the inspector.");
            GameOverScreen = new GameObject("GameOverScreen", typeof(GameOver));
            GameOverScreen.SetActive(false);
        }
        MeleeAttacks[0].GetComponent<PlayerAttackMelee>().range = attackRange;
        MeleeAttacks[0].GetComponent<PlayerAttackMelee>().doesStun = false;

        RangedAttacks[0].GetComponent<PlayerAttackRanged>().maxDistance = attackRange;
        RangedAttacks[0].GetComponent<PlayerAttackRanged>().doesStun = true;

        playerSprites = funcInst.SliceGrid(playerTexture, 64, 76);

        // Initializing values
        moveInputVector2 = Vector2.zero;
        moveInput = Vector3.zero;
        playerVel = Vector3.zero;
        
        playerTransform = GetComponent<Transform>();
        playerPos = playerTransform.position;

        facing = VarTypes.Direction.Down;

        data.velocity = playerVel;
        data.position = playerPos;
        data.health = health;
        data.direction = facing;

        currentSprite = playerSprites[6];
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        GameOverScreen.SetActive(false);
        GameOverScreen.transform.position = playerPos;

        transform.Find("Main Camera").position = playerPos + new Vector3(0, 0, -10);
        GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    // Update is called once per frame
    public void Update()
    {
        if(cheatMode)
        {
            baseMult = 1000;
        }
        if(!cheatMode)
        {
            baseMult = 1;
        }

        if (Time.time - timeAtLastSpriteUpdate > 0.33f)
        {
            timeAtLastSpriteUpdate = Time.time;
            currentSpriteIndexAdd += animationDirectionMultiplier;
            if (currentSpriteIndexAdd >= 2 || currentSpriteIndexAdd <= 0)
            {
                animationDirectionMultiplier *= -1;
            }
        }

        // Setting current-frame constants
        deltaTime = Time.deltaTime;
        moveAction = moveActionReference.action;

        HandleInputs();
    
        // Move Input Vector
        moveInputVector2 = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        moveInput = (Vector3)moveInputVector2;

        inventory = GlobalVars.items;

        UpdateMovement();

        UpdatePlayerFacingDirection();
        nearbyLivingEnemies = GetNearbyLivingEnemies();
        DrawCircle(attackRange, transform.position, 0.01f);
    }

    VarTypes.EnemyData[] GetNearbyEnemies()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>();
        VarTypes.EnemyData[] livingEnemiesList = new VarTypes.EnemyData[enemies.Length];
        if (enemies.Length == 0)
        {
            return livingEnemiesList.ToArray();
        }

        //Debug.Log(enemies.Length);
        VarTypes.EnemyData currentEnemy = new VarTypes.EnemyData();
        int numOfRealEnemies = 0;
        int currentIndex = 0;
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isFarAway) continue;
            if (enemy == null) continue;
            numOfRealEnemies++;
            //Debug.Log($"{enemy.name} is not far away");
            currentEnemy.gameObject = enemy.gameObject;
            currentEnemy.distance = Vector3.Distance(this.transform.position, enemy.transform.position);

            livingEnemiesList[currentIndex] = currentEnemy;
            currentIndex++;
        }

        VarTypes.EnemyData[] nearbyLivingEnemies = new VarTypes.EnemyData[numOfRealEnemies];
        for (int i = 0; i < numOfRealEnemies; i++)
        {
            nearbyLivingEnemies[i] = livingEnemiesList[i];
        }

        return nearbyLivingEnemies;
    }

    VarTypes.EnemyData[] GetNearbyLivingEnemies()
    {
        VarTypes.EnemyData[] enemyDatas = GetNearbyEnemies();
        EnemyController[] nearbyEnemies = new EnemyController[enemyDatas.Length];
        for(int i = 0; i < enemyDatas.Length; i++)
        {
            nearbyEnemies[i] = enemyDatas[i].gameObject.GetComponent<EnemyController>();
        }
        VarTypes.EnemyData[] livingEnemiesList = new VarTypes.EnemyData[enemyDatas.Length];
        if(nearbyEnemies.Length == 0)
        {
            return livingEnemiesList;
        }

        EnemyController enemy;
        for (int i = 0; i < nearbyEnemies.Length; i++)
        {
            enemy = nearbyEnemies[i];


            livingEnemiesList[i] = new VarTypes.EnemyData(enemy.gameObject, Vector3.Distance(transform.position, enemy.transform.position));
        }

        VarTypes.EnemyData[] nearbyLivingEnemies = livingEnemiesList.ToArray();

        return nearbyLivingEnemies;
    }
    
    void RangedAttack(GameObject enemy, int attackIndex)
    {
        if (!cheatMode && Time.time - timeAtLastAttack < attackCooldown)
        {
            Debug.LogAssertion("not enough time, dumbass");
            return;
        }

        timeAtLastAttack = Time.time;

        GameObject playerAttack = new GameObject("Ranged Attack", typeof(PlayerAttackRanged), typeof(SpriteRenderer));
        playerAttack.GetComponent<PlayerAttackRanged>().maxDistance = baseMult * attackRange;
        playerAttack.GetComponent<PlayerAttackRanged>().damage = baseMult * RangedAttacks[attackIndex].GetComponent<PlayerAttackRanged>().damage;
        playerAttack.GetComponent<SpriteRenderer>().sprite = RangedAttacks[attackIndex].GetComponent<SpriteRenderer>().sprite;
        playerAttack.GetComponent<PlayerAttackRanged>().speed = RangedAttacks[attackIndex].GetComponent<PlayerAttackRanged>().speed;
        playerAttack.GetComponent<PlayerAttackRanged>().doesStun = RangedAttacks[attackIndex].GetComponent<PlayerAttackRanged>().doesStun;
        playerAttack.transform.localScale = RangedAttacks[attackIndex].transform.localScale;
        playerAttack.GetComponent<PlayerAttackRanged>().OnAttack(enemy, playerPos);
        playerAttack.SetActive(true);

        //enemy.GetComponent<EnemyController>().OnHit(damage);

        Vector2 VectorFromPlayerToEnemy = enemy.transform.position - playerPos;
        VectorFromPlayerToEnemy.Normalize();
    }

    void MeleeAttack(GameObject enemy, int attackIndex)
    {
        if (!cheatMode && Time.time - timeAtLastAttack < attackCooldown) return;
        timeAtLastAttack = Time.time;
        GameObject playerAttack = new GameObject("Melee Attack", typeof(PlayerAttackMelee), typeof(SpriteRenderer));
        playerAttack.GetComponent<PlayerAttackMelee>().swingSpeed = MeleeAttacks[attackIndex].GetComponent<PlayerAttackMelee>().swingSpeed;
        playerAttack.GetComponent<PlayerAttackMelee>().swingAngle = MeleeAttacks[attackIndex].GetComponent<PlayerAttackMelee>().swingAngle;
        playerAttack.GetComponent<PlayerAttackMelee>().range = baseMult * attackRange;
        playerAttack.GetComponent<PlayerAttackMelee>().doesStun = MeleeAttacks[attackIndex].GetComponent<PlayerAttackMelee>().doesStun;
        playerAttack.GetComponent<SpriteRenderer>().sprite = MeleeAttacks[attackIndex].GetComponent<SpriteRenderer>().sprite;
        playerAttack.GetComponent<PlayerAttackMelee>().damage = baseMult * MeleeAttacks[attackIndex].GetComponent<PlayerAttackMelee>().damage;
        playerAttack.transform.localScale = MeleeAttacks[attackIndex].transform.localScale;
        playerAttack.GetComponent<PlayerAttackMelee>().OnAttack(enemy, playerPos);
        //playerAttack.SetActive(true);
        //enemy.GetComponent<EnemyController>().OnHit(damage);
        Vector2 VectorFromPlayerToEnemy = enemy.transform.position - playerPos;
        VectorFromPlayerToEnemy.Normalize();
    }

    void HandleInputs()
    {
        if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.qKey.wasPressedThisFrame)
        {
            string debugImageName = "Hide-the-pain-Harold-large-meme-8_0";
            transform.Find(debugImageName).gameObject.SetActive(!transform.Find(debugImageName).gameObject.activeSelf);
        }

        if(Time.time - timeAtLastDash > dashCooldown)
        {
            if (Keyboard.current.shiftKey.isPressed)
            {
                playerVel += dashSpeed * maxSpeed * moveInput;
                timeAtLastDash = Time.time;
            }
        }

        Vector2 MouseInWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float MouseDistanceFromPlayer = Vector2.Distance(MouseInWorldPosition, playerPos);
        if (MouseDistanceFromPlayer <= baseMult * attackRange)
        {
            if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
            {
                //Debug.LogAssertion("Player Attacked!");
                if(attackIndex == -1)
                {
                    attackIndex = 0;
                }
                //Debug.LogAssertion($"{attackIndex}");
                float minDistanceFromMouse = float.PositiveInfinity;
                int indexOfMinDistance = -1;
                for (int i = 0; i < nearbyLivingEnemies.Length; i++)
                {
                    float currDistFromMouse = Vector3.Distance(MouseInWorldPosition, nearbyLivingEnemies[i].gameObject.transform.position);
                    if (currDistFromMouse > baseMult * attackLenience) continue;
                    if (nearbyLivingEnemies[i].distance > attackRange) continue;
                    if (minDistanceFromMouse > currDistFromMouse)
                    {
                        minDistanceFromMouse = currDistFromMouse;
                        indexOfMinDistance = i;
                    }
                }
                //Debug.LogAssertion($"{indexOfMinDistance}");
                //Debug.LogAssertion($"{minDistanceFromMouse}");
                if (indexOfMinDistance == -1) return;

                bool isRangedAttack = Mouse.current.leftButton.isPressed && cheatMode || Mouse.current.leftButton.wasPressedThisFrame && !cheatMode;
                bool isMeleeAttack = Mouse.current.rightButton.isPressed && cheatMode || Mouse.current.rightButton.wasPressedThisFrame && !cheatMode;
                if (isRangedAttack)
                {
                    RangedAttack(nearbyLivingEnemies[indexOfMinDistance].gameObject, attackIndex);
                }
                if (isMeleeAttack)
                {
                    MeleeAttack(nearbyLivingEnemies[indexOfMinDistance].gameObject, attackIndex);
                }
            }
        }
    }

    void UpdateMovement()
    {
        AccelerationMovement();

        UpdatePosition();
        data.velocity = playerVel;
        data.position = playerPos;
        float epsilon = 0.001f;

        if (Math.Abs(playerVel.y) < Math.Abs(playerVel.x))
        {
            if (playerVel.x > epsilon)
            {
                facing = VarTypes.Direction.Right;
            }
            else if(playerVel.x < -epsilon)
            {
                facing = VarTypes.Direction.Left;
            }
            else
            {
                facing = VarTypes.Direction.Down;
            }
        }
        else
        {
            if (playerVel.y > epsilon)
            {
                facing = VarTypes.Direction.Up;
            }
            else if (playerVel.y < -epsilon)
            {
                facing = VarTypes.Direction.Down;
            }
            else
            {
                facing = VarTypes.Direction.Down;
            }
        }

        data.direction = facing;
        GameOverScreen.transform.position = playerPos;
    }

    void UpdatePlayerFacingDirection()
    {
        VarTypes.Direction facing = data.direction;
        if (facing == VarTypes.Direction.Left)
        {
            currentSprite = playerSprites[3 + currentSpriteIndexAdd];
        }
        if (facing == VarTypes.Direction.Right)
        {
            currentSprite = playerSprites[6 + currentSpriteIndexAdd];
        }
        if (facing == VarTypes.Direction.Up)
        {
            currentSprite = playerSprites[9 + currentSpriteIndexAdd];
        }
        if (facing == VarTypes.Direction.Down)
        {
            currentSprite = playerSprites[0 + currentSpriteIndexAdd];
        }
        playerTransform.GetComponent<SpriteRenderer>().sprite = currentSprite;
    }
    
    // Updating the player's acceleration
    void AccelerationMovement()
    {
        Vector3 currentAcc = playerAcc * moveInput;
    
        playerVel += deltaTime * (currentAcc - Functions.Friction(playerVel, playerAcc / maxSpeed));
    }
    void UpdatePosition()
    {
        playerPos += deltaTime * playerVel;

        data.position = playerPos;
        data.velocity = playerVel;
        data.transform = transform;
        data.direction = facing;
        data.health = health;

        transform.SetPositionAndRotation(playerPos, playerTransform.rotation);
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
            float sinAngle = Mathf.Sin(angle * i);
            float cosAngle = Mathf.Cos(angle * i);
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(cosAngle, sinAngle, 0, 0),
                                                     new Vector4(-sinAngle, cosAngle, 0, 0),
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

    public void OnHit(int damage)
    {
        int dealtDamage = damage - defense;
        dealtDamage = Math.Max(0, dealtDamage);

        health -= dealtDamage;
        data.health = health;
        timeAtLastHit = Time.time;

        //Debug.Log($"Player took {dealtDamage} damage. Health is now {health}. Time since last hit: {Time.time - timeAtLastHit}");

        if (health <= 0 && canDie && !cheatMode)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
