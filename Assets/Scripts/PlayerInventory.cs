using Assets.Globals;
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;

public class PlayerInventory : MonoBehaviour
{
    public List<VarTypes.Item> items = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        items.Clear();
        Debug.Log("list count = " + items.Count);
    }

    public void AddItemToInventory(VarTypes.Item item)
    {
        AddItemToInventory(item.id);
    }

    public void AddItemToInventory(int itemID)
    {
        for(int i = 0; i < GlobalVars.items.Length; i++)
        {
            if (GlobalVars.items[i].id == itemID)
            {
                AddItemToInventory(GlobalVars.items[i]);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.hKey.wasReleasedThisFrame)
        {
            Debug.Log(items.Count);
            foreach (VarTypes.Item item in items)
            {
                Debug.Log(item.name);
            }
        }
    }
}
