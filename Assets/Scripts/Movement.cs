// Unity
using Unity;
using Unity.Mathematics;
using Unity.VisualScripting;

// Unity Engine
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Transform))]
public class Movement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionReference;

	[SerializeField] private float maxSpeed = 5f;
	[SerializeField] private float playerAcc = 5f;
    [SerializeField] private float frictionCoefficient = 0.001f;
    [SerializeField] private bool useAcceleration = true;
	private Vector3 playerVel;
    private InputAction moveAction;
	private Transform playerPos;
    private float dt;

    private Vector2 moveInputVector2;
    private Vector3 moveInput;
    // Start is called before the first frame update
    void Start()
    {
        // preliminary checks
        if (moveActionReference == null)
        {
            Debug.LogError("no InputActionReference");
            return;
        }

        // initialize values
        moveInputVector2 = Vector2.zero;
        moveInput = Vector3.zero;
        playerVel = Vector3.zero;

        playerPos = GetComponent<Transform>();
    }

	// Update is called once per frame
	void Update()
    {
        dt = Time.deltaTime;
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
        // Movement in metres per second

        Vector3 currentVel = maxSpeed * moveInput;


        // Multiplying by deltaTime to make framerate not affect speed
        
        playerVel = dt * currentVel;
        
    }

    // Updating the player's acceleration
	void AccelerationMovement()
	{
        Vector3 currentAcc = playerAcc * moveInput;

        playerVel += dt * currentAcc;

        if (playerVel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            playerVel = playerVel.normalized * maxSpeed;
        }

        playerVel = Friction(playerVel, frictionCoefficient);
    }

    void UpdatePosition()
    {
        playerPos.position += dt * playerVel;
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
        return vel * (1 - fricCoef);
    }
}