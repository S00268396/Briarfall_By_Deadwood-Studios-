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
            //Call the new smart function that handles stacking and UI
            manager.AddItem(itemToGive, amount);

            Debug.Log($"Picked up {amount}{itemToGive.name}");

            Destroy(gameObject);
        }
    }
}
