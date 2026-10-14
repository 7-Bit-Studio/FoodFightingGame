using UnityEngine;

public class EnemyDrop : MonoBehaviour
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropTheShit(Vector3 pos, Quaternion rotation)
    {
        transform.gameObject.SetActive(true);
        Instantiate(this, pos, rotation);
        Debug.Log("Object has Spawned!");
       
    }


}
