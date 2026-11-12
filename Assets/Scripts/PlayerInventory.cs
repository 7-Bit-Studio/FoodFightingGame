using Assets.Globals;
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;

public class PlayerInventory : MonoBehaviour
{
    [Serializable] 
    public struct Inventory
    {
        public VarTypes.Item item;
        public int count;
        public Inventory(VarTypes.Item item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }

    public Inventory[] inventory = new Inventory[GlobalVars.items.Length];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < GlobalVars.items.Length; i++)
        {
            inventory[i].item = GlobalVars.items[i];
            inventory[i].count = 0;
        }
    }

    public void AddItemToInventory(VarTypes.Item item)
    {
        AddItemToInventory(item.id);
    }

    public void AddItemToInventory(int itemID)
    {
        inventory[itemID].count++;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
