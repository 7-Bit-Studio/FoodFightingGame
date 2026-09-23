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
    
public class Movement : MonoBehaviour
{
    public struct Data
    {
        public Vector3 playerVel;
        public Transform playerPos;
    }

    [Header("Movement Settings")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float playerAcc = 5f;
    [SerializeField] private float frictionCoefficient = 1.01f;
    [SerializeField] private bool useAcceleration = true;
    
    // Player values
    private Transform playerPos;
    private Vector3 playerVel;
    
    // PlayerData Values
    private VarTypes.Direction facing;
    private Vector2 moveInputVector2;
    private InputAction moveAction;
    private Vector3 moveInput;
    private Data data;
    public Data GetData() => data;

    // DeltaTime
    private float deltaTime;

    void Awake()
    {
        // Preliminary checks
        if (moveActionReference == null)
        {
            Debug.LogError("no InputActionReference");
            return;
        }
    
        // Initializing values
        moveInputVector2 = Vector2.zero;
        moveInput = Vector3.zero;
        playerVel = Vector3.zero;
    
        playerPos = GetComponent<Transform>();
    
        data.playerVel = playerVel;
        data.playerPos = playerPos;
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Setting current-frame constants
        deltaTime = Time.deltaTime;
        moveAction = moveActionReference.action;
    
        // Move Input Vector
    
        moveInputVector2 = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        moveInput = (Vector3)moveInputVector2;


        if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.qKey.wasReleasedThisFrame)
        {
            string debugImageName = "Hide-the-pain-Harold-large-meme-8_0";
            transform.Find(debugImageName).gameObject.SetActive(!transform.Find(debugImageName).gameObject.activeSelf);
        }



        UpdateMovement();

        UpdatePlayerFacingDirection();
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
        data.playerVel = playerVel;
        data.playerPos = playerPos;

        if(playerVel.x > 0)
        {
            facing = VarTypes.Direction.Left;
        }
        else
        {
            facing = VarTypes.Direction.Right;
        }
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
}
