// System

// Unity
using UnityEngine;
using UnityEngine.U2D;

// Globals
using Assets.Globals;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private Texture2D enemyTexture;
    public int health = 100;
    [SerializeField] private int defense = 0;
    [SerializeField] private float acceleration = 0.2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackDelay = 100;
    [SerializeField] private int collisionLoops = 1;
    [SerializeField] private GameObject theDrop;
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private int textureWidth = 64;
    [SerializeField] private int textureHeight = 76;
    [SerializeField] private float enterDetectionRadius = 10f;
    [SerializeField] private float exitDetectionRadius = 15f;
    public int dropItemID = 0;
    private bool hasDetectedPlayer = false;

    // EnemyController values
    private Transform enemyTransform;
    private Vector3 enemyPos;
    private Vector3 enemyVel;

    // EnemyData Values
    private VarTypes.Direction facing;
    private VarTypes.Data data;
    private VarTypes.Data playerData;

    private Sprite[] enemySprites;
    private Sprite currentSprite;
    private int currentSpriteIndex;

    // DeltaTime
    private float deltaTime;
    private float timeAtLastSpriteUpdate;
    private float timeAtLastAttack;

    private Vector3 playerDirection;
    private Vector3 normalizedPlayerDirection;
    private GameObject player;
    private CraftingMenu cMenu;
    public bool isStunned;
    private bool wasJustHit;
    private float hitTime;
    private GameObject healthBar;
    private Transform healthValueTransform;
    private int startHealth;
    public bool isFarAway = false;
    private bool canMove;


    private bool isAlive;
    private Functions funcs = new();
    private FunctionsInstance funcInst = new();
    private readonly SpriteFilter spriteFilter = new();
    private bool hasDrop = false;
    private bool shouldStunPseudoGlobal = false;
    public VarTypes.Data GetData() => data;
    private PlayerController playerController;
    void Start()
    {
        wasJustHit = false;
        isStunned = false;
        player = funcInst.GetSiblingGameObject("Player");
        playerController = player.GetComponent<PlayerController>();
        cMenu = player.GetComponent<CraftingMenu>();

        playerData = playerController.GetData();

        enemyTransform = GetComponent<Transform>();
        enemyPos = enemyTransform.position;
        data.velocity = enemyVel;
        data.transform = enemyTransform;
        data.health = health;
        data.position = enemyPos;

        enemySprites = funcInst.SliceGrid(enemyTexture, textureWidth, textureHeight);
        enemySprites = spriteFilter.RemoveEmpty(enemyTexture, enemySprites);
        timeAtLastSpriteUpdate = Time.time;
        timeAtLastAttack = Time.time;
        currentSpriteIndex = 0;

        startHealth = health;

        isAlive = true;

        canMove = true;

        if (isBoss)
        {
            healthBar = transform.Find("HealthBarBoss").gameObject;
            healthValueTransform = healthBar.transform.Find("HealthBarInner");
        }

        if(theDrop != null)
        {
            hasDrop = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.gameObject.activeSelf) return;
        if (health <= 0)
        {
            Kill();
            return;
        }

        canMove = !cMenu.active;

        Vector2 ScreenPos = Camera.main.WorldToScreenPoint(enemyPos);
        ScreenPos.x /= Camera.main.pixelWidth;
        ScreenPos.y /= Camera.main.pixelHeight;
        ScreenPos *= 2;
        ScreenPos -= Vector2.one;

        //Debug.Log($"Screen Position: {ScreenPos}");
        if (Mathf.Abs(ScreenPos.x) > 3 || Mathf.Abs(ScreenPos.y) > 3) isFarAway = true;
        else isFarAway = false;


        if (isFarAway) return;

        playerData = playerController.GetData();

        deltaTime = Time.deltaTime;

        if (Vector3.Distance(enemyPos, playerData.position) < enterDetectionRadius)
        {
            playerDirection = playerData.position - transform.position;
            normalizedPlayerDirection = playerDirection.normalized;
            hasDetectedPlayer = true;
        }
        
        if (Vector3.Distance(enemyPos, playerData.position) > exitDetectionRadius)
        {
            hasDetectedPlayer = false;
        }


        if (Time.time - timeAtLastSpriteUpdate > animationDuration)
        {
            currentSprite = enemySprites[currentSpriteIndex];
            timeAtLastSpriteUpdate = Time.time;
            GetComponent<SpriteRenderer>().sprite = currentSprite;
            currentSpriteIndex += 1;
            currentSpriteIndex %= enemySprites.Length;
        }


        if (isBoss)
        {
            // if anyone knows of a better way to do this, please let me know
            // no, setting localScale.x to a number doesn't work
            // same with localPosition
            healthValueTransform.localScale = new Vector3(health * 0.9f / startHealth, healthValueTransform.localScale.y, healthValueTransform.localScale.z);
            healthValueTransform.localPosition = new Vector3((0.9f * ((float)health / startHealth) - 0.9f) / 2f, healthValueTransform.localPosition.y, healthValueTransform.localPosition.z);
        }

        UpdateMovement();
    }

    public void OnHit(int damage, bool shouldStun)
    {
        int dealtDamage = damage - defense;
        dealtDamage = dealtDamage < 0 ? 0 : dealtDamage;

        health -= dealtDamage;

        //Debug.Log($"{transform.name} was hit for {damage} damage!");

        enemyVel = -normalizedPlayerDirection * 3;
        hitTime = Time.time;
        wasJustHit = true;
        shouldStunPseudoGlobal = shouldStun;

        if (health <= 0 && isAlive)
        {
            //Debug.Log($"{transform.name} has died!");
            //Drop the relevant item for the enemy
            Kill();
        }
    }
    void Kill()
    {
        if (hasDrop)
        {
            theDrop.GetComponent<EnemyDrop>().DropTheShit(transform.position, transform.rotation, theDrop.GetComponent<EnemyDrop>().itemID);
        }
        isAlive = false;

        enemyTransform.gameObject.SetActive(false);
        Destroy(enemyTransform.gameObject, 0.5f);
    }
    void UpdateMovement()
    {

        if (canMove)
        {
            if (wasJustHit)
            {
                //Debug.Log($"Enemy was just hit. Stunned: {isStunned}, Should Stun: {shouldStunPseudoGlobal}, Time since hit: {Time.time - hitTime}");
                if (Time.time - hitTime > 0.25f)
                {
                    wasJustHit = false;
                    isStunned = shouldStunPseudoGlobal;
                }


                UpdatePosition();
                return;
            }

            if (isStunned)
            {
                //Debug.Log($"Enemy is stunned. Stunned: {isStunned}, Should Stun: {shouldStunPseudoGlobal}, Time since hit: {Time.time - hitTime}");
                if (Time.time - hitTime > 1f)
                {
                    isStunned = false;
                    enemyVel = Vector3.zero;
                }

                enemyVel *= 0.98f;

                UpdatePosition();
                return;
            }


            if (maxSpeed == 0)
            {
                //Debug.LogWarning($"{transform.name}: Max speed is set to 0. Enemy will not move.");
                enemyVel = Vector3.zero;
                return;
            }


            if (hasDetectedPlayer)
            {
                enemyVel += ((acceleration * normalizedPlayerDirection) - Functions.Friction(enemyVel, acceleration / maxSpeed)) * deltaTime;
            }
            else
            {
                enemyVel -= Functions.Friction(enemyVel, acceleration / maxSpeed) * deltaTime;
            }

            data.velocity = enemyVel;
            data.position = enemyPos;
            data.transform = enemyTransform;

            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        enemyPos += enemyVel * deltaTime;
        if (isStunned)
        {
            Debug.Log($"Enemy Position: {enemyPos}, Enemy Velocity: {enemyVel}, DeltaTime: {deltaTime}");
        }

        for (int i = 0; i < collisionLoops; i++)
        {
            Collisions();
        }

        enemyTransform.position = enemyPos;

        data.velocity = enemyVel;
        data.position = enemyPos;
        data.transform = enemyTransform;

        // leaving enemyTransform.position to keep function looking balanced
        transform.SetPositionAndRotation(enemyPos, enemyTransform.rotation);
    }

    void EnemyCollision()
    {
        int arrOffset = 0;
        var objects = FindObjectsByType<EnemyController>();
        float colliderRad = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

        Vector3 colliderOffset = (Vector3)GetComponent<CircleCollider2D>().offset * transform.localScale.x;
        Vector3 colliderCenter;
        GameObject[] gameObjects = new GameObject[objects.Length];

        for(int i = 0; i < objects.Length; i++)
        {
            if (objects[i].name == transform.name)
            {
                arrOffset = 1;
                continue;
            }
            gameObjects[i - arrOffset] = objects[i].gameObject;
        }

        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject == null) continue;
            colliderCenter = enemyPos + colliderOffset;
            //Debug.Log($"Checking collision");
            float otherColliderRad = gameObject.GetComponent<CircleCollider2D>().radius * gameObject.transform.localScale.x;
            Vector3 otherColliderCenter = gameObject.transform.position + (Vector3)gameObject.GetComponent<CircleCollider2D>().offset * gameObject.transform.localScale.x;

            float currentDistance = Vector3.Distance(colliderCenter, otherColliderCenter);
            float minDistance = colliderRad + otherColliderRad;

            if (currentDistance < minDistance)
            {
                enemyPos += (colliderCenter - otherColliderCenter) * (minDistance - currentDistance);
                enemyVel += (colliderCenter - otherColliderCenter) * (minDistance - currentDistance);
                //Debug.Log("Collision!");
            }

            // leaving enemyTransform.position to keep function looking balanced
            transform.SetPositionAndRotation(enemyTransform.position, enemyTransform.rotation);
        }
    }

    /// <summary>
    /// CALLED EVERY FRAME, BE CAREFUL
    /// </summary>
    void PlayerCollision()
    {
        if (!player.GetComponent<CircleCollider2D>().isActiveAndEnabled) return;

        Vector3 colliderOffset = (Vector3)GetComponent<CircleCollider2D>().offset * transform.localScale.x;
        Vector3 colliderCenter;
        Vector3 playerColliderOffset = (Vector3)player.GetComponent<CircleCollider2D>().offset * player.transform.localScale.x;
        Vector3 playerColliderCenter;
        float colliderRad = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

        colliderCenter = enemyPos + colliderOffset;
        playerColliderCenter = player.transform.position + playerColliderOffset;
        float playerColliderRad = player.GetComponent<CircleCollider2D>().radius * player.transform.localScale.x;
        float currentDistance = Vector3.Distance(colliderCenter, playerColliderCenter);
        float minDistance = colliderRad + playerColliderRad;

        if (currentDistance < minDistance)
        {
            Debug.Log($"{playerDirection}");
            Debug.Log($"Player Distance: {playerDirection.magnitude}, Player Collider Radius: {playerColliderRad}, Enemy Collider Radius: {colliderRad}, Total Collider Radius: {colliderRad + playerColliderRad}");
            // normalizedPlayerDirection is negative to make vector point away from player
            enemyPos += (colliderCenter - playerColliderCenter) * (minDistance - currentDistance);
            enemyVel += (colliderCenter - playerColliderCenter) * (minDistance - currentDistance);

            Debug.Log("Player Collision!");

            if (Time.time - timeAtLastAttack > attackDelay)
            {
                timeAtLastAttack = Time.time;
                playerController.OnHit(damage);
            }
        }

        // leaving enemyTransform.position to keep function looking balanced
        transform.SetPositionAndRotation(enemyTransform.position, enemyTransform.rotation);
    }

    /// <summary>
    /// Resolve all collisions with enemies and the player
    /// </summary>
    void Collisions()
    {
        EnemyCollision();
        PlayerCollision();
    }
}