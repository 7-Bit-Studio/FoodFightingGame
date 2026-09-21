// Unity
using Unity;
using Unity.Mathematics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    /// <summary>
    /// Data about the player
    /// </summary>
    public struct PlayerData
    {
        /// <summary>
        /// player velocity
        /// </summary>
        public Vector3 playerVel;
        /// <summary>
        /// player position
        /// </summary>
        public Transform playerPos;

    }
    [RequireComponent(typeof(Transform))]
    
    public class Movement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private InputActionReference moveActionReference;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float playerAcc = 5f;
        [SerializeField] private float frictionCoefficient = 0.001f;
        [SerializeField] private bool useAcceleration = true;

        // Player values
        private PlayerData playerData;
        private Transform playerPos;
        private Vector3 playerVel;

        // PlayerData Values
        private InputAction moveAction;
        private Vector2 moveInputVector2;
        private Vector3 moveInput;

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
            playerData.playerPos = playerPos;
            playerData.playerVel = playerVel;
        }

        // Update is called once per frame
        void Update()
        {
            // Setting current-frame constants
            deltaTime = Time.deltaTime;
            moveAction = moveActionReference.action;

            // Move Input Vector

            moveInputVector2 = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            moveInput = ((Vector3)moveInputVector2).normalized;

            if (useAcceleration)
            {
                AccelerationMovement();
            }
            else
            {
                VelocityMovement();
            }

            UpdatePosition();

            Debug.Log($"{playerVel.x} {playerVel.y} {playerVel.z}");
        }

        // Updateing the player's velocity
        void VelocityMovement()
        {
            // PlayerData in metres per second

            Vector3 currentVel = maxSpeed * moveInput * 500;


            // Multiplying by deltaTime to make framerate not affect speed

            playerVel = deltaTime * currentVel;
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

            playerVel = Friction(playerVel, frictionCoefficient);
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
            return vel / deltaTime * fricCoef;
        }
    }
}