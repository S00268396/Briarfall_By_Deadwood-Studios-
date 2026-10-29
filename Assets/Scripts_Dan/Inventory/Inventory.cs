using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] public List<InventoryItem> Items;

    public Inventory(List<InventoryItem> items)
    {
        Items = items;
    }

    [Serializable]
    public class Save
    {
        public List<InventoryItem.Save> Items = new();
    }

    public Save GetSave()
    {
        Save save = new Save();

        foreach(var item in Items)
        {
            save.Items.Add(item.GetSave());
        }
        return save;
    }
}
