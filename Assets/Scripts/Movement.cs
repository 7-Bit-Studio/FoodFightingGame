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

	[SerializeField] private float playerAcc = 5f;
	private Vector3 playerVel;
	private float maxSpeed;
    private InputAction moveAction;
	private Transform playerPos;
    // Start is called before the first frame update
    void Start()
	{
		playerPos = GetComponent<Transform>();
    }

	// Update is called once per frame
	void Update()
    {
        float dt = Time.deltaTime;

        if (moveActionReference == null)
		{
			Debug.LogError("no InputActionReference");
			return;
		}

		moveAction = moveActionReference.action;

		if(moveAction == null)
		{
			Debug.LogError("no InputAction");
			return;
		}

		Vector3 moveInput = moveAction != null ? moveAction.ReadValue<Vector3>() : Vector2.zero;

		if(moveInput == null)
		{
			Debug.LogError("no MoveInput provided");
			return;
		}
		playerVel += dt * playerAcc * moveInput;
		if (playerVel.sqrMagnitude > maxSpeed * maxSpeed)
		{
			playerVel = playerVel.normalized * maxSpeed;
		}
        playerPos.position += dt * playerVel;

        /*
		 * Set the player's position, then rotation
		 * Can be simplified to the line below
		 * 
		 * transform.SetPositionAndRotation(playerPos.position, playerPos.rotation);
		 */

        transform.position = playerPos.position;
		transform.rotation = playerPos.rotation;

		//Debug.Log($"Movement\n\t(x,y)\n\t({moveInput.x},{moveInput.y})");
	}
}