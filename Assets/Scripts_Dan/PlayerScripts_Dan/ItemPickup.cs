using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public BaseTemData itemToGive;
    public int amount = 1;

    public void CollectItem()
    {
        //find the iventory manager in the screen
        InventoryManager manager = FindAnyObjectByType<InventoryManager>();

        if(manager != null && manager.InventoryData != null )
        {
            //added the item to the player data list
            manager.InventoryData.Inventory.Items.Add(new InventoryItem(itemToGive, amount));

            Debug.Log($"Picked up {amount}{itemToGive.name}");

            Destroy(gameObject);
        }
    }
}
