using Assets.Globals;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{




    [SerializeField] private GameObject theItem;

    [SerializeField] private PlayerInventory pInventory;
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
            

            pInventory = collision.GetComponent<PlayerInventory>();

            Debug.Log(pInventory.items.Count);
            pInventory.items.Add(theItem);

            Debug.Log(pInventory.items.Count);

            transform.gameObject.SetActive(false);
            
        }
    }

    
}
