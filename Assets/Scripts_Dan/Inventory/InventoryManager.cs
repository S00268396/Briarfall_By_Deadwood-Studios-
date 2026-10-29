using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryData InventoryData;

    public ItemTable itemDatabase;

     void Start()
    {
        Load();
    }
    public void Save()
    {
        if (InventoryData != null)
            return;

        Inventory.Save save = InventoryData.Inventory.GetSave();
        string json = JsonUtility.ToJson(save, true);

        //Changed to persistentDataPath so it saves correctly in built, playable games 
       string savePath = Path.Combine(Application.persistentDataPath, $"{InventoryData.name}.json");
        File.WriteAllText(savePath, json);

        Debug.Log($"Inventory saved to {savePath}");
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
}


