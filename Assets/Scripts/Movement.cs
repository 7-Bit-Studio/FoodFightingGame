using Unity;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
public class Movement : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionReference;

	[SerializeField] private float playerVel = 0.5f;
	[SerializeField] private Text textElement;
    private InputAction moveAction;
	private Transform playerPos;
	private float fpsLow;
    // Start is called before the first frame update
    void Start()
	{
		playerPos = GetComponent<Transform>();
		fpsLow = float.PositiveInfinity;
    }

	// Update is called once per frame
	void Update()
	{
		
		float dt = Time.deltaTime;
		float fps = 1 / dt;

		fpsLow = Mathf.Min(fps, fpsLow);

		textElement.text = $"{fps}fps\n{fpsLow}fps min";
		
		if(moveActionReference == null)
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

		Vector3 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector3.zero;

		if(moveInput == null)
		{
			Debug.LogError("no MoveInput provided");
			return;
		}
		

		playerPos.position += dt * playerVel * moveInput;

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