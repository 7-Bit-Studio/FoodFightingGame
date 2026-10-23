// System
using System;
// Custom functions and variable types
using Assets.Globals;
// Unity
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;
using System.Linq;


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
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float playerAcc = 5f;
    [SerializeField] private float frictionCoefficient = 1.01f;
    [SerializeField] private bool useAcceleration = true;
    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private int defense = 0;
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private GameObject[] RangedAttacks;
    [SerializeField] private GameObject[] MeleeAttacks;
    [SerializeField] private bool CheatMode = false;
    [SerializeField] private float attackLenience = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    
    
    // PlayerData Values
    private InputAction moveAction;
    public VarTypes.Data GetData() => data;
    private VarTypes.Direction facing;
    private VarTypes.Data data;
    private Vector2 moveInputVector2;
    private Vector3 moveInput;
    private Transform playerTransform;
    private Vector3 playerPos;
    private Vector3 playerVel;

    // DeltaTime
    private float deltaTime;
    private GameObject[] livingEnemies;
    private float[] enemyDistances;
    private LineRenderer lineRenderer;
    private float timeAtLastAttack;
    private readonly Functions functions = new();
    private int animationDirectionMultiplier = 1;

    // Player Animation
    private Sprite[] playerSprites;
    private Sprite currentSprite;
    private int currentSpriteIndexAdd;
    private float timeAtLastSpriteUpdate;
    private int attackIndex;
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

        playerSprites = functions.SliceGrid(playerTexture, 64, 76);

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

        transform.Find("Main Camera").localPosition = new Vector3(0, 0, -10);
        //GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    // Update is called once per frame
    public void Update()
    {

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
    
    void RangedAttack(GameObject enemy, int attackIndex)
    {
        if (!CheatMode && Time.time - timeAtLastAttack < attackCooldown) return;

        timeAtLastAttack = Time.time;

        GameObject playerAttack = new();
        playerAttack.AddComponent<PlayerAttackRanged>();
        playerAttack.GetComponent<PlayerAttackRanged>().maxDistance = attackRange;
        playerAttack.AddComponent<SpriteRenderer>();
        playerAttack.GetComponent<SpriteRenderer>().sprite = RangedAttacks[attackIndex].GetComponent<SpriteRenderer>().sprite;
        playerAttack.transform.localScale = RangedAttacks[attackIndex].transform.localScale;
        playerAttack.GetComponent<PlayerAttackRanged>().OnAttack(enemy, playerPos);
        playerAttack.SetActive(true);

        //enemy.GetComponent<EnemyController>().OnHit(damage);

        Vector2 VectorFromPlayerToEnemy = enemy.transform.position - playerPos;
        VectorFromPlayerToEnemy.Normalize();
    }

    void MeleeAttack(GameObject enemy, int attackIndex)
    {
        if (!CheatMode && Time.time - timeAtLastAttack < attackCooldown) return;
        timeAtLastAttack = Time.time;
        GameObject playerAttack = new();
        playerAttack.AddComponent<PlayerAttackMelee>();
        playerAttack.GetComponent<PlayerAttackMelee>().swingSpeed = MeleeAttacks[0].GetComponent<PlayerAttackMelee>().swingSpeed;
        playerAttack.GetComponent<PlayerAttackMelee>().swingAngle = MeleeAttacks[0].GetComponent<PlayerAttackMelee>().swingAngle;
        playerAttack.AddComponent<SpriteRenderer>();
        playerAttack.GetComponent<SpriteRenderer>().sprite = MeleeAttacks[0].GetComponent<SpriteRenderer>().sprite;
        playerAttack.transform.localScale = MeleeAttacks[0].transform.localScale;
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
        if ((Mouse.current.rightButton.wasPressedThisFrame && !CheatMode) || (CheatMode && Mouse.current.rightButton.isPressed))
        {
            Vector2 MouseInWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            float minDistanceFromMouse = float.PositiveInfinity;
            int indexOfMinDistance = -1;

            for (int i = 0; i < livingEnemies.Length; i++)
            {
                float currDistFromMouse = Vector3.Distance(MouseInWorldPosition, livingEnemies[i].transform.position);
                if (currDistFromMouse > attackLenience) continue;
                if (minDistanceFromMouse > currDistFromMouse)
                {
                    minDistanceFromMouse = currDistFromMouse;
                    indexOfMinDistance = i;
                }
            }

            //Debug.Log($"{mouseDistFromPlayer}");

            if (indexOfMinDistance == -1) return;

            //Debug.Log($"{indexOfMinDistance}");
            MeleeAttack(livingEnemies[indexOfMinDistance], attackIndex);
        }
        if ((Mouse.current.leftButton.wasPressedThisFrame && !CheatMode) || (CheatMode && Mouse.current.leftButton.isPressed))
        {
            Vector2 MouseInWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            float minDistanceFromMouse = float.PositiveInfinity;
            int indexOfMinDistance = -1;

            for (int i = 0; i < livingEnemies.Length; i++)
            {
                float currDistFromMouse = Vector3.Distance(MouseInWorldPosition, livingEnemies[i].transform.position);
                if (currDistFromMouse > attackLenience) continue;
                if (minDistanceFromMouse > currDistFromMouse)
                {
                    minDistanceFromMouse = currDistFromMouse;
                    indexOfMinDistance = i;
                }
            }

            //Debug.Log($"{mouseDistFromPlayer}");

            if (indexOfMinDistance == -1) return;

            //Debug.Log($"{indexOfMinDistance}");
            RangedAttack(livingEnemies[indexOfMinDistance], attackIndex);
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
        if(Math.Abs(playerVel.x) < Math.Abs(playerVel.y))
        {
            if (playerVel.y > epsilon)
            {
                facing = VarTypes.Direction.Up;
            }
            else if(playerVel.y < -epsilon)
            {
                facing = VarTypes.Direction.Down;
            }
            else
            {
                facing = VarTypes.Direction.Down;
            }
        }

        data.direction = facing;
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
    
    // Updating the player's velocity
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
        playerPos += deltaTime * playerVel;

        data.position = playerPos;
        data.velocity = playerVel;
        data.transform = transform;
        data.direction = facing;
        data.health = health;

        transform.SetPositionAndRotation(playerPos, playerTransform.rotation);
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

    public void OnHit(int damage)
    {
        int dealtDamage = damage - defense;
        dealtDamage = Math.Max(0, dealtDamage);

        health -= dealtDamage;

        if(health <= 0)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void OnCollision(GameObject @object)
    {
        //assuming box collider
        if (@object.GetComponent<Collider>().GetType() != typeof(BoxCollider2D)) return;

        Collider2D boxCollider2D = @object.GetComponent<Collider2D>();
        Bounds bounds = boxCollider2D.bounds;
        float horizontalDisplacement = bounds.extents.x - Math.Abs(playerPos.x - bounds.center.x);
        float verticalDisplacement = bounds.extents.y - Math.Abs(playerPos.y - bounds.center.y);

        if (horizontalDisplacement > 0) return;

        if (verticalDisplacement > 0) return;

        if (verticalDisplacement < horizontalDisplacement)
        {
            playerPos.y += (bounds.center.y - playerPos.y > 0 ? 1 : -1) * (playerPos.y - verticalDisplacement);
        }
        else
        {
            playerPos.x += (bounds.center.x - playerPos.x > 0 ? 1 : -1) * (playerPos.x - horizontalDisplacement);
        }
    }
}
