using System.ComponentModel;
using System;

// Unity
using Unity;
using Unity.U2D;
using Unity.Mathematics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;

// Globals
using Assets.Globals;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private int health = 100;
    [SerializeField] private int defense = 0;
    [SerializeField] private float acceleration = 0.2f;
    /// <summary>
    /// How many seconds for each animation frame to last
    /// </summary>
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private int collisionLoops = 1;
    [SerializeField] private bool isBoss = false;

    // EnemyController values
    private Transform enemyPos;
    private Vector3 enemyVel;

    // EnemyData Values
    private VarTypes.Direction facing;
    private VarTypes.Data data;
    private VarTypes.Data playerData;
    private PlayerController movement;

    private Sprite[] spriteArr;
    private Sprite currentSprite;
    private int currentSpriteIndex;

    // DeltaTime
    private float deltaTime;
    private float timeAtLastSpriteUpdate;

    private Vector3 playerDirection;
    private Vector3 normalizedPlayerDirection;
    private GameObject player;
    private bool isStunned;
    private bool wasJustHit;
    private float hitTime;
    private GameObject healthBar;
    private Transform healthValueTransform;
    private int startHealth;
    private float inverseStartHealth;

    public VarTypes.Data GetData() => data; 
    void Start()
    {
        wasJustHit = false;
        isStunned = false;
        player = Functions.GetSiblingGameObject("Player");

        movement = player.GetComponent<PlayerController>();

        playerData = movement.GetData();

        enemyPos = GetComponent<Transform>();
        data.velocity = enemyVel;
        data.position = enemyPos;
        data.health = health;

        spriteArr = new Sprite[spriteAtlas.spriteCount];
        timeAtLastSpriteUpdate = Time.time;
        currentSpriteIndex = 0;
        spriteAtlas.GetSprites(spriteArr);

        startHealth = health;
        inverseStartHealth = 1 / startHealth;

        if (isBoss)
        {
            healthBar = transform.Find("HealthBarBoss").gameObject;
            healthValueTransform = healthBar.transform.Find("HealthBarInner");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.gameObject.activeSelf) return;
        playerData = movement.GetData();
        
        playerDirection = playerData.position.transform.position - transform.position;
        normalizedPlayerDirection = playerDirection.normalized;
        deltaTime = Time.deltaTime;

        if (Time.time - timeAtLastSpriteUpdate > animationDuration)
        {
            currentSprite = spriteArr[currentSpriteIndex];
            timeAtLastSpriteUpdate = Time.time;
            GetComponent<SpriteRenderer>().sprite = currentSprite;
            currentSpriteIndex += 1;
            currentSpriteIndex %= spriteArr.Length;
        }


        if (isBoss)
        {
            healthValueTransform.localScale = new Vector3(health * 0.9f / startHealth, healthValueTransform.localScale.y, healthValueTransform.localScale.z);
            healthValueTransform.localPosition = new Vector3(( 0.9f * ((float)health / startHealth) - 0.9f)/2f, healthValueTransform.localPosition.y, healthValueTransform.localPosition.z);
        }

        UpdateMovement();
    }

    public void OnHit(int damage)
    {
        int dealtDamage = damage - defense;
        dealtDamage = dealtDamage < 0 ? 0 : dealtDamage;

        health -= dealtDamage;

        Debug.Log($"{transform.name} was hit for {damage} damage!");

        enemyVel = -normalizedPlayerDirection * 3;
        hitTime = Time.time;
        wasJustHit = true;

        if(health <= 0)
        {
            transform.gameObject.SetActive(false);
            Debug.Log($"{transform.name} has died!");
        }
    }   
    void UpdateMovement()
    {
        if (wasJustHit)
        {
            if (Time.time - hitTime > 0.25f)
            {
                wasJustHit = false;
                isStunned = true;
            }


            UpdatePosition();
            return;
        }

        if (isStunned)
        {
            if(Time.time - hitTime > 1f) isStunned = false;

            enemyVel *= 0.95f;

            UpdatePosition();
            return;
        }

        enemyVel += acceleration * deltaTime * normalizedPlayerDirection;

        if (enemyVel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            enemyVel = enemyVel.normalized * maxSpeed;
        }

        data.velocity = enemyVel;
        data.position = enemyPos;

        UpdatePosition();
    }

    void UpdatePosition()
    {
        enemyPos.position += enemyVel * deltaTime;

        for (int i = 0; i < collisionLoops; i++)
        {
            Collisions();
        }

        data.velocity = enemyVel;
        data.position = enemyPos;

        transform.SetPositionAndRotation(enemyPos.position, enemyPos.rotation);
    }

    void EnemyCollision()
    {
        int arrOffset = 0;
        var objects = FindObjectsByType<EnemyController>();
        float colliderRad = GetComponent<CircleCollider2D>().radius;

        Vector3 colliderOffset = (Vector3)GetComponent<CircleCollider2D>().offset;
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
            colliderCenter = enemyPos.position + colliderOffset;
            //Debug.Log($"Checking collision");
            float otherColliderRad = gameObject.GetComponent<CircleCollider2D>().radius;
            Vector3 otherColliderCenter = gameObject.transform.position + (Vector3)gameObject.GetComponent<CircleCollider2D>().offset;

            float currentDistance = Vector3.Distance(colliderCenter, otherColliderCenter);
            float minDistance = colliderRad + otherColliderRad;

            if (currentDistance < otherColliderRad + colliderRad)
            {
                enemyPos.position += (colliderCenter - otherColliderCenter) * (minDistance - currentDistance);
                enemyVel += (colliderCenter - otherColliderCenter) * (minDistance - currentDistance);
                Debug.Log("Collision!");
            }
            transform.SetPositionAndRotation(enemyPos.position, enemyPos.rotation);
        }
    }

    /// <summary>
    /// CALLED EVERY FRAME, BE CAREFUL
    /// </summary>
    void PlayerCollision()
    {
        if (!player.GetComponent<CircleCollider2D>().isActiveAndEnabled) return;

        float colliderRad = GetComponent<CircleCollider2D>().radius;

        float playerColliderRad = player.GetComponent<CircleCollider2D>().radius;

        if (playerDirection.magnitude < colliderRad + playerColliderRad)
        {
            enemyPos.position += -normalizedPlayerDirection * ((colliderRad + playerColliderRad) - playerDirection.magnitude);
            enemyVel += -playerDirection * ((colliderRad + playerColliderRad) - playerDirection.magnitude);
            Debug.Log("Player Collision!");
        }

        transform.SetPositionAndRotation(enemyPos.position, enemyPos.rotation);
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
