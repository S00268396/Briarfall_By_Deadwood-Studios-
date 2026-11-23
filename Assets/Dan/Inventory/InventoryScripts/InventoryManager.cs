using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryData InventoryData;
    public ItemTable itemDatabase;

    [Header("UI Slots")]
    public Slot[] hotbarSlots;      // your 6 bottom hotbar slots
    public Slot[] inventorySlots;   // your main inventory panel slots

    [Header("Save Options")]
    [Tooltip("ON: delete the saved inventory every time the game starts, so each run begins empty. Turn OFF to keep progress between runs.")]
    public bool startFresh = true;

    void Start()
    {
        ValidateSlots();

        // Make sure the data object exists
        if (InventoryData != null)
        {
            if (InventoryData.Inventory == null)
                InventoryData.Inventory = new Inventory(new List<InventoryItem>());
            else if (InventoryData.Inventory.Items == null)
                InventoryData.Inventory.Items = new List<InventoryItem>();

            // 1. wipe leftover "ghost" data from the Unity editor
            InventoryData.Inventory.Items.Clear();
        }

        // 2. new game: delete the old save file. Otherwise: load it.
        if (startFresh)
        {
            if (InventoryData != null)
            {
                string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");

                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                    Debug.Log("startFresh is on: old inventory save deleted.");
                }
            }
        }
        else
        {
            Load();
        }

        // 3. draw the UI
        UpdateInventoryUI();
    }

    // Logs exactly which slot references are broken so you don't have to hunt for them
    void ValidateSlots()
    {
        var seen = new HashSet<Slot>();
        CheckSlots(hotbarSlots, "hotbarSlots", seen);
        CheckSlots(inventorySlots, "inventorySlots", seen);
    }

    void CheckSlots(Slot[] slots, string label, HashSet<Slot> seen)
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError($"{label} is empty. Assign slots or use 'Auto Assign Slots'.", this);
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                Debug.LogError($"{label}[{i}] is EMPTY. This is what stops later slots from drawing.", this);
            else if (!seen.Add(slots[i]))
                Debug.LogError($"{label}[{i}] ({slots[i].name}) is assigned more than once.", slots[i]);
        }
    }

    public void Save()
    {
        if (InventoryData == null || InventoryData.Inventory == null)
            return;

        Inventory.Save save = InventoryData.Inventory.GetSave();
        string json = JsonUtility.ToJson(save, true);
        string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");

        File.WriteAllText(savePath, json);   // this line was missing before
    }

    public void Load()
    {
        if (InventoryData == null || itemDatabase == null)
        {
            Debug.LogWarning("InventoryData or ItemDatabase is missing on the InventoryManager!");
            return;
        }

        string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");

        // if the file doesn't exist (first playthrough), stop loading
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found. Starting with default inventory");
            return;
        }

        string json = File.ReadAllText(savePath);
        Inventory.Save save = JsonUtility.FromJson<Inventory.Save>(json);

        if (save == null || save.Items == null)
            return;

        List<InventoryItem> items = new List<InventoryItem>();

        foreach (var item in save.Items)
        {
            BaseTemData data = itemDatabase.Find(item.ID);

            if (data != null)
                items.Add(new InventoryItem(data, item.CurrentStackSize));
            else
                Debug.LogWarning($"Saved item ID {item.ID} was not found in the ItemTable, skipping it.", itemDatabase);
        }

        InventoryData.Inventory = new Inventory(items);
        Debug.Log("Inventory loaded successfully.");
    }

    public void AddItem(BaseTemData itemToAdd, int amount)
    {
        if (itemToAdd == null || InventoryData == null || InventoryData.Inventory == null)
            return;

        var items = InventoryData.Inventory.Items;
        int remaining = amount;

        // 1. top up existing stacks of this item
        foreach (var item in items)
        {
            if (item.Item == itemToAdd && item.CurrentStackSize < itemToAdd.MaxStackSize)
            {
                int spaceLeft = itemToAdd.MaxStackSize - item.CurrentStackSize;
                int amountToAdd = Mathf.Min(spaceLeft, remaining);

                item.CurrentStackSize += amountToAdd;
                remaining -= amountToAdd;

                if (remaining <= 0)
                    break;
            }
        }

        // 2. put whatever is left into new slots (respecting the max stack size)
        int totalSlotCount = hotbarSlots.Length + inventorySlots.Length;

        while (remaining > 0 && items.Count < totalSlotCount)
        {
            int stack = Mathf.Min(Mathf.Max(1, itemToAdd.MaxStackSize), remaining);
            items.Add(new InventoryItem(itemToAdd, stack));
            remaining -= stack;
        }

        if (remaining > 0)
            Debug.Log("Inventory is full!");

        // 3. update the UI and save if we picked up at least some of it
        if (remaining < amount)
        {
            UpdateInventoryUI();
            Save();
        }
    }

    public void UpdateInventoryUI()
    {
        if (InventoryData == null || InventoryData.Inventory == null)
            return;

        var items = InventoryData.Inventory.Items;

        RefreshSlots(hotbarSlots, items);
        RefreshSlots(inventorySlots, items);
    }

    void RefreshSlots(Slot[] slots, List<InventoryItem> items)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            // one broken slot must not stop the rest from drawing
            if (slots[i] == null)
                continue;

            slots[i].UpdateSlot(i < items.Count ? items[i] : null);
        }
    }
}