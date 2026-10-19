using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public List<GameObject> items = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (items.Count > 0) {
            foreach (GameObject item in items) {
                Debug.Log(item);
            }
        }
    }
}
