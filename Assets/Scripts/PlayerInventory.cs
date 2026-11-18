using Assets.Globals;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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

    public Inventory[] inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        inventory =  new Inventory[GlobalVars.items.Length]; //moved into start as doing it before that sets inventory to size 0


        for (int i = 0; i < GlobalVars.items.Length; i++)
        {
            inventory[i].item = GlobalVars.items[i];
            inventory[i].count = 0;
        }

        
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
