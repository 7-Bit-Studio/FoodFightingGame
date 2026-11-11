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
        GameObject droppedItem = new GameObject("Dropped Item", typeof(EnemyDrop), typeof(CircleCollider2D));
        GameObject sprite;
        if (droppedItem.transform.childCount == 0)
        {
            sprite = new GameObject("Sprite", typeof(SpriteRenderer));
        }
        else
        {
            sprite = droppedItem.transform.Find("Sprite").gameObject;
        }

        if (!funcInst.HasComponent<SpriteRenderer>(sprite))
        {
            sprite.AddComponent<SpriteRenderer>();
        }

        sprite.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        sprite.transform.localScale = Vector3.one / 2;
        sprite.transform.parent = droppedItem.transform;

        droppedItem.GetComponent<EnemyDrop>().itemID = itemID;
        droppedItem.GetComponent<CircleCollider2D>().radius = gameObject.GetComponent<CircleCollider2D>().radius;
        droppedItem.tag = "Item";

        droppedItem.transform.SetPositionAndRotation(position, rotation);
        droppedItem.name = $"{GlobalVars.items[itemID].name}";
        Debug.Log("Object has Spawned!");
       
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() == null) { return; }
        Debug.Log("Trigger Detected!");

        pInventory = collision.GetComponent<PlayerInventory>();

        Debug.Log(pInventory.items.Count);
        pInventory.items.Add(GlobalVars.items[itemID]);
        Destroy(this.gameObject);
    }
}
