using Assets.Globals;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;
using Assets.Globals;

public class PlayerInventory : MonoBehaviour
{
    public List<VarTypes.Item> items = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        items.Clear();
        Debug.Log("list count = " + items.Count);
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
