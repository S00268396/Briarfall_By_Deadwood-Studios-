using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class InventoryItem 
{
    public BaseTemData Item;
    public int CurrentStackSize;

    public InventoryItem() { }

    public InventoryItem(BaseTemData item, int currentStackSize)
    {
        Item = item;
        CurrentStackSize = currentStackSize;
    }

    [Serializable]
    public class Save
    {
        public string ID;
        public int CurrentStackSize;
    }

    public Save GetSave()
    {
        return new Save()
        {
            ID = Item.ID,
            CurrentStackSize = CurrentStackSize
        };
    }
}
