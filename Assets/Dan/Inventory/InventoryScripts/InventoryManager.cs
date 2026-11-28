using System.Collections.Generic;
using System.IO;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   

public class InventoryManager : MonoBehaviour
{
    public InventoryData InventoryData;
    public ItemTable itemDatabase;

    [Header("UI Slots")]
    public Slot[] hotbarSlots;      // your 6 bottom hotbar slots
    public Slot[] inventorySlots;   // your main inventory panel slots

    [Header("Drag & Drop UI")]
    public Image dragIcon;      //Drag your DragItem UI Image here

    [Header("Save Options")]
    [Tooltip("ON: delete the saved inventory every time the game starts, so each run begins empty. Turn OFF to keep progress between runs.")]
    public bool startFresh = true;

    private int dragFromIndex = -1;
    private bool isDragging = false;

    void Start()
    {
        ValidateSlots();
        AssignSlotIndices();

        // Make sure the data object exists
        if (InventoryData != null)
        {
            if (InventoryData.Inventory == null)
                InventoryData.Inventory = new Inventory(new List<InventoryItem>());
            else if (InventoryData.Inventory.Items == null)
                InventoryData.Inventory.Items = new List<InventoryItem>();

            // 1. wipe leftover "ghost" data from the Unity editor
            InventoryData.Inventory.Items.Clear();
            EnsureFixedSlotCount();
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

    void Update()
    {
        HandleDragInput();
    }

    void AssignSlotIndices()
    {
        if (hotbarSlots != null)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
                if (hotbarSlots[i] != null) hotbarSlots[i].slotIndex = i;
        }

        int offset = hotbarSlots != null ? hotbarSlots.Length : 0;
        if (inventorySlots != null)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
                if (inventorySlots[i] != null) inventorySlots[i].slotIndex = offset + i;
        }
    }

    //Ensures inventory.Items always has 18 slots
    void EnsureFixedSlotCount()
    {
        if(inventorySlots == null ||  inventorySlots.Length == null) return;

        int totalSlots = (hotbarSlots !=null ? hotbarSlots.Length :0) +
                          (inventorySlots != null ? inventorySlots.Length : 0);

        var items = InventoryData.Inventory.Items;
        while (items.Count < totalSlots)
            items.Add(new InventoryItem(null, 0));
                        
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

        foreach (var savedItem in save.Items)
        {
            //Keep empty slots in their saved positions
            if(string.IsNullOrEmpty(savedItem.ID))
            {
                items.Add(new InventoryItem(null, 0));
                continue;
            }

            BaseTemData data = itemDatabase.Find(savedItem.ID);
            if (data != null)
                items.Add(new InventoryItem(data, savedItem.CurrentStackSize));
            else
                items.Add(new InventoryItem(null, 0));
        }

        InventoryData.Inventory = new Inventory(items);
        EnsureFixedSlotCount();
        Debug.Log("Inventory loaded successfully.");
    }

    //-------------------------------------------------------------
    //1. PICKUPROUTING (Tools -> Hotbar, Other Items -> Main Bag)
    //-------------------------------------------------------------
    public void AddItem(BaseTemData itemToAdd, int amount)
    {
        if (itemToAdd == null || InventoryData == null || InventoryData.Inventory == null)
            return;

        EnsureFixedSlotCount();
        int remaining = amount;
        int hotbarCount = hotbarSlots.Length;
        int totalSlotCount = hotbarCount + inventorySlots.Length;

       if(itemToAdd.isTool)
        {
            //Tools check hotbar (0..5) first, then Main Inventory(6..17)
            remaining = TryFillRange(itemToAdd, remaining, 0, hotbarCount);
            if (remaining > 0)
                remaining = TryFillRange(itemToAdd, remaining, hotbarCount, totalSlotCount);
        }
       else
        {
            //Normal items check Main Inventory(6..17) first, then Hotbar(0..5)
            remaining = TryFillRange(itemToAdd, remaining, hotbarCount, totalSlotCount);
            if(remaining > 0)
                remaining = TryFillRange(itemToAdd, remaining, 0,  totalSlotCount);
        }

        if (remaining > 0)
            Debug.Log("Inventory is Full!");

        if(remaining < amount)
        {
            UpdateInventoryUI();
            Save();
        }
    }

    private int TryFillRange(BaseTemData itemToAdd, int remaining, int startIndex, int endIndex)
    {
        var items = InventoryData.Inventory.Items;

        //First pass: top up exisitng stacks in this range
        for(int i = startIndex; i < endIndex; i++)
        {
            if (!items[i].isEmpty && items[i].Item == itemToAdd && items[i].CurrentStackSize < itemToAdd.MaxStackSize)
            {
                int spaceLeft = itemToAdd.MaxStackSize - items[i].CurrentStackSize;
                int amountToAdd = Mathf.Min(spaceLeft, remaining);

                items[i].CurrentStackSize += amountToAdd;
                remaining -= amountToAdd;

                if (remaining <= 0)
                    return 0;
            }
        }

        //Second pass: place into the first empty slot in this range
        for(int i = startIndex; i < endIndex; i++)
        {
            if (items[i].isEmpty)
            {
                int stack = Mathf.Min(Mathf.Max(1, itemToAdd.MaxStackSize), remaining);
                items[i] = new InventoryItem(itemToAdd, stack);
                remaining -= stack;

                if(remaining <= 0)
                    return 0;
            }
        }

        return remaining;
    }

    // ---------------------------------------------------------
    // 2. DRAG, SWAP, AND STACK LOGIC
    // ---------------------------------------------------------
    private void HandleDragInput()
    {
        if (InventoryData?.Inventory?.Items == null || Cursor.lockState == CursorLockMode.Locked)
            return;

        var items = InventoryData.Inventory.Items;

        if (Input.GetMouseButtonDown(0))
            TryBeginDrag(items);

        if (isDragging && dragIcon != null)
            dragIcon.transform.position = Input.mousePosition;

        if (Input.GetMouseButtonUp(0) && isDragging)
            TryEndDrag(items);
    }

    private void TryBeginDrag(List<InventoryItem> items)
    {
        Slot hovered = GethoveredSlot();
        if (hovered == null || hovered.slotIndex < 0 || hovered.slotIndex >= items.Count)
            return;

        InventoryItem clickedItem = items[hovered.slotIndex];
        if (clickedItem.isEmpty)
            return;

        dragFromIndex = hovered.slotIndex;
        isDragging = true;

        if (dragIcon == null)
        {
            Debug.LogWarning("Drag Icon is not assigned on InventoryManager!", this);
            return;
        }

        dragIcon.sprite = clickedItem.Item.Icon;
        dragIcon.color = new Color(1f, 1f, 1f, 0.6f);
        dragIcon.transform.SetAsLastSibling();
        dragIcon.enabled = true;
    }

    private void TryEndDrag(List<InventoryItem> items)
    {
        Slot hovered = GethoveredSlot();
        if (hovered != null && hovered.slotIndex >= 0 && hovered.slotIndex < items.Count)
            HandleDrop(dragFromIndex, hovered.slotIndex);

        CancelDrag();
    }

    public void CancelDrag()
    {
        if(dragIcon != null)
            dragIcon.enabled = false;
        dragFromIndex = -1;
        isDragging = false;
    }

    private Slot GethoveredSlot()
    {
        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult hit in results)
            {
                Slot[] slots = hit.gameObject.GetComponentsInParent<Slot>();
                if (slots != null && slots.Length > 0)
                    return slots[slots.Length - 1];
            }
        }
        return null;
    }

    private void HandleDrop(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        var items = InventoryData.Inventory.Items;
        var fromItem = items[fromIndex];
        var toItem = items[toIndex];

        // Case 1: Target slot is empty -> Move item
        if (toItem.isEmpty)
        {
            items[toIndex] = fromItem;
            items[fromIndex] = new InventoryItem(null, 0);
        }
        // Case 2: Target slot holds the same item -> Stack up to MaxStackSize
        else if (toItem.Item == fromItem.Item)
        {
            int spaceLeft = toItem.Item.MaxStackSize - toItem.CurrentStackSize;
            if (spaceLeft > 0)
            {
                int moveAmount = Mathf.Min(spaceLeft, fromItem.CurrentStackSize);
                toItem.CurrentStackSize += moveAmount;
                fromItem.CurrentStackSize -= moveAmount;

                if (fromItem.CurrentStackSize <= 0)
                    items[fromIndex] = new InventoryItem(null, 0);
            }
        }
        // Case 3: Target slot holds a different item -> Swap them
        else
        {
            items[toIndex] = fromItem;
            items[fromIndex] = toItem;
        }

        UpdateInventoryUI();
        Save();
    }

    // ---------------------------------------------------------
    // 3. REFRESH UI SLOTS (Hotbar = 0..5, Inventory = 6..17)
    // ---------------------------------------------------------
    public void UpdateInventoryUI()
    {
        if (InventoryData == null || InventoryData.Inventory == null)
            return;

        EnsureFixedSlotCount();
        var items = InventoryData.Inventory.Items;

        RefreshSlots(hotbarSlots, items, 0);
        RefreshSlots(inventorySlots, items, hotbarSlots.Length);
    }

    void RefreshSlots(Slot[] slots, List<InventoryItem> items, int startOffset)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            int dataIndex = startOffset + i;
            InventoryItem itemToDraw = (dataIndex < items.Count && !items[dataIndex].isEmpty)
                ? items[dataIndex]
                : null;

            slots[i].UpdateSlot(itemToDraw);
        }
    }
}