using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryData InventoryData;
    public ItemTable itemDatabase;

    [Header("UI Slots")]
    public Slot[] hotbarSlots;      //assign your 6 bottom hotbar slots here
    public Slot[] inventorySlots;   //assign your main inventory panel slots here

     void Start()
    {

        //1 wipe leftover "ghost" data from the Unity editor
        if(InventoryData != null && InventoryData.Inventory != null && InventoryData.Inventory.Items != null)
        {
            InventoryData.Inventory.Items.Clear();
        }

        //2 load the actual JSON save file(if one exists)
        Load();

        //3 Draw the UI
        UpdateInventoryUI();
    }

    public void Save()
    {
        if (InventoryData == null)
            return;
        Inventory.Save save = InventoryData.Inventory.GetSave();
        string json = JsonUtility.ToJson(save, true);
        string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");
    }

    public void Load()
    {
       if(InventoryData == null ||  itemDatabase == null)
        {
            Debug.LogWarning("InventoryData or ItemDatabase is missing on the InventoryManager!");
            return;
        }

        string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");

        //safely check: if the file doesnt exist(like on a first playthrough), stop loading
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found. Starting with default inventory");
            return;
        }

        string json = File.ReadAllText(savePath);
        Inventory.Save save = JsonUtility.FromJson<Inventory.Save>(json);

        List<InventoryItem> items = new List<InventoryItem>();
        
        foreach(var item in save.Items)
        {
            BaseTemData data = itemDatabase.Find(item.ID);

            if(data != null)
            {
                items.Add(new InventoryItem(data, item.CurrentStackSize));
            }
        }

        InventoryData.Inventory = new Inventory(items);
        Debug.Log("Inventory loaded successfully.");
    }

    public void AddItem(BaseTemData itemToAdd, int amount)
    {
        int remaining = amount;

        //1 check if we have this item already and can stack it
        foreach(var item in InventoryData.Inventory.Items)
        {
            if(item.Item == itemToAdd && item.CurrentStackSize < itemToAdd.MaxStackSize)
            {
                int spaceLeft = itemToAdd.MaxStackSize - item.CurrentStackSize;
                int amountToAdd = Mathf.Min(spaceLeft, remaining);

                item.CurrentStackSize += amountToAdd;
                remaining -= amountToAdd;

                if (remaining <= 0)
                    break;
            }
        }

        //2 if there's leftover, add to a new slot
        int totalSlotCount = hotbarSlots.Length + inventorySlots.Length;
        if(remaining > 0)
        {
            if(InventoryData.Inventory.Items.Count < totalSlotCount)
            {
                InventoryData.Inventory.Items.Add(new InventoryItem(itemToAdd, remaining));
                remaining = 0;
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }

        //3 Update the UI and Save the game
        if(remaining < amount)    // Means we successfully picked up at least some of it
        {
            UpdateInventoryUI();
            Save();
        }
    }

    public void UpdateInventoryUI()
    {
        int totalItems = InventoryData.Inventory.Items.Count;

        //fill Hotbar Slots (the first 6 items)
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if(i < totalItems)
            {
                hotbarSlots[i].UpdateSlot(InventoryData.Inventory.Items[i]);
            }
            else
            {
                hotbarSlots[i].ClearSlot();
            }
        }

        //then fill the inventory slots 
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(i < totalItems)
            {
                inventorySlots[i].UpdateSlot(InventoryData.Inventory.Items[i]);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }
}


