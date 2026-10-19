using Assets.Globals;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{

    private Transform itemTransform;
    
    private GameObject player;
    private PlayerController movement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
