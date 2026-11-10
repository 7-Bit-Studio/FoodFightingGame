using Assets.Globals;
using UnityEngine;
public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject theItem;
    [SerializeField] private PlayerInventory pInventory;
    private int count;
    private readonly Functions funcs = new();
    private readonly FunctionsInstance funcInst = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pInventory = funcInst.GetSiblingGameObject("Player").GetComponent<PlayerInventory>();
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
        if (collision.GetComponent<PlayerController>() == null) { return; }
        Debug.Log("Trigger Detected!");

        pInventory = collision.GetComponent<PlayerInventory>();

        count = pInventory.items.Count;

        Debug.Log(pInventory.items.Count);
        pInventory.items.Add(theItem);

        Debug.Log(pInventory.items.Count);
        Debug.Log(pInventory.items[count]);

        funcInst.Kill(transform.gameObject);
    }
}
