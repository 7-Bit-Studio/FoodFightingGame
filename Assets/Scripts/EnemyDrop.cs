using Assets.Globals;
using Unity.VisualScripting;
using UnityEngine;
public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject theItem;
    [SerializeField] private PlayerInventory pInventory;
    public int itemID;
    private readonly Functions funcs = new();
    private readonly FunctionsInstance funcInst = new();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pInventory = funcInst.GetSiblingGameObject("Player").GetComponent<PlayerInventory>();
    }

    //The code to spawn the item in the world
    public void DropTheShit(Vector3 position, Quaternion rotation, int itemID)
    {
        //Spawn the object
        GameObject droppedItem = new GameObject("Dropped Item", typeof(EnemyDrop), typeof(CircleCollider2D), typeof(Rigidbody2D));
        GameObject sprite = new GameObject("Sprite", typeof(SpriteRenderer));

        sprite.transform.parent = droppedItem.transform;
        sprite.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        sprite.transform.localScale = Vector3.one / 2;

        droppedItem.GetComponent<EnemyDrop>().itemID = itemID;
        droppedItem.GetComponent<CircleCollider2D>().radius = gameObject.GetComponent<CircleCollider2D>().radius * 0.5f;
        droppedItem.GetComponent<CircleCollider2D>().isTrigger = true;
        droppedItem.GetComponent<Rigidbody2D>().gravityScale = 0;
        droppedItem.tag = "Item";

        droppedItem.transform.SetPositionAndRotation(position, rotation);
        droppedItem.name = $"{GlobalVars.items[itemID].name}";
        //Debug.Log("Object has Spawned!");
       
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collided!!");
        if (collision.GetComponent<PlayerController>() == null) { return; }

        //collision.GetComponent<PlayerInventory>().AddItemToInventory(itemID);

        pInventory = collision.GetComponent<PlayerInventory>();

        //Debug.Log(pInventory.items.Count);
        pInventory.AddItemToInventory(itemID);
        Destroy(this.gameObject);
    }
}
