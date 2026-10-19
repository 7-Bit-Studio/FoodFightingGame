using Assets.Globals;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{

    private Transform itemTransform;
    private Vector3 itemPos;
    private Vector3 itemVel;

    [SerializeField] private int collisionLoops = 1;

    private Vector3 playerDirection;
    private Vector3 normalizedPlayerDirection;
    private GameObject player;
    private VarTypes.Data playerData;
    private PlayerController movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Functions.GetSiblingGameObject("Player");
        movement = player.GetComponent<PlayerController>();

        playerData = movement.GetData();

        itemTransform = GetComponent<Transform>();
        itemPos = itemTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //The code to spawn the item in the world
    public void DropTheShit(Vector3 pos, Quaternion rotation)
    {
        //Spawn the object
        Instantiate(this, pos, rotation);
        Debug.Log("Object has Spawned!");
       
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Trigger Detected!");
            transform.gameObject.SetActive(false);
        }
    }

    
}
