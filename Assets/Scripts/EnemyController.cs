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
    [SerializeField] private float enemyAcc = 25f;
    [SerializeField] private float frictionCoefficient = 1.01f;
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private SpriteAtlas spriteAtlas;
    /// <summary>
    /// How many seconds for each animation frame to last
    /// </summary>
    [SerializeField] private float animationDuration = 0.25f;

    // Player values
    private Transform enemyPos;
    private Vector3 enemyVel;

    // PlayerData Values
    private VarTypes.Direction facing;
    private VarTypes.Data data;
    private Movement movement;

    private Sprite[] spriteArr;
    private Sprite currentSprite;
    private int currentSpriteIndex;

    // DeltaTime
    private float deltaTime;
    private float timeAtLastSpriteUpdate;

    private Vector3 playerDirection;
    private Vector3 normalizedPlayerDirection;
    private Scene scene;
    private GameObject[] gameObjects;
    private GameObject player;

    void Start()
    {
        player = Functions.GetSiblingGameObject(transform, "Player");

        movement = player.GetComponent<Movement>();

        data = movement.GetData();

        enemyPos = GetComponent<Transform>();
        data.velocity = enemyVel;
        data.position = enemyPos;

        spriteArr = new Sprite[spriteAtlas.spriteCount];
        timeAtLastSpriteUpdate = Time.time;
        currentSpriteIndex = 0;
        spriteAtlas.GetSprites(spriteArr);

        Debug.Log(spriteArr.Length);
    }

    // Update is called once per frame
    void Update()
    {
        data = movement.GetData();
        
        playerDirection = data.position.transform.position - transform.position;
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

        UpdateMovement();
    }

    void UpdateMovement()
    {
        enemyVel = maxSpeed * playerDirection;

        if (enemyVel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            enemyVel = enemyVel.normalized * maxSpeed;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        enemyPos.position += enemyVel * deltaTime;

        Collisions();

        transform.rotation = enemyPos.rotation;
        transform.position = enemyPos.position;
    }

    void EnemyCollision()
    {
        int arrOffset = 0;
        var objects = FindObjectsByType<EnemyController>();
        float colliderRad = GetComponent<CircleCollider2D>().radius;
        Vector3 colliderOffset = (Vector3)GetComponent<CircleCollider2D>().offset;
        Vector3 colliderCenter = enemyPos.position + colliderOffset;
        GameObject[] gameObjects = new GameObject[objects.Length - 1];
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
            float otherColliderRad = gameObject.GetComponent<CircleCollider2D>().radius;
            Vector3 otherColliderCenter = gameObject.transform.position + (Vector3)gameObject.GetComponent<CircleCollider2D>().offset;

            if (Vector3.Distance(colliderCenter,otherColliderCenter) < otherColliderRad + colliderRad)
            {
                enemyPos.position += (colliderCenter - otherColliderCenter) * ((otherColliderRad + colliderRad) - Vector3.Distance(colliderCenter, otherColliderCenter));
            }
        }
    }

    void PlayerCollision()
    {
        float colliderRad = GetComponent<CircleCollider2D>().radius;
        Vector3 colliderOffset = (Vector3)GetComponent<CircleCollider2D>().offset;
        Vector3 colliderCenter = enemyPos.position + colliderOffset;

        Vector3 playerColliderPos = (Vector3)player.GetComponent<CircleCollider2D>().offset + data.position.transform.position;
        float playerColliderRad = player.GetComponent<CircleCollider2D>().radius;


        if (Vector3.Distance(colliderCenter, playerColliderPos) < colliderRad + playerColliderRad)
        {
            enemyPos.position += -playerDirection * ((colliderRad + playerColliderRad) - playerDirection.magnitude);
            Debug.Log("Collision!");
        }
    }

    void Collisions()
    {
        EnemyCollision();
        PlayerCollision();
    }
}
