using UnityEngine;
using UnityEngine.InputSystem;

public class ToolPickup : MonoBehaviour
{
    [SerializeField] private ToolType toolToGrant = ToolType.Sickle;
    [SerializeField] private BaseTemData itemData;  //Drag item_sickle here
    [SerializeField] private bool autoPickupOnWalkOver = false; // True = walk over to grab, False = press E

    private PlayerToolHandler playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerToolHandler>(out PlayerToolHandler handler))
        {
            playerInRange = handler;

            if (autoPickupOnWalkOver)
            {
                PickUp();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerToolHandler>(out PlayerToolHandler handler) && handler == playerInRange)
        {
            playerInRange = null;
        }
    }

    private void Update()
    {
        // Simple 'E' key check so you don't have to touch InputManager or InventoryManager
        if (!autoPickupOnWalkOver && playerInRange != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        playerInRange.EquipTool(toolToGrant);

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if(inventoryManager != null && itemData != null)
        {
            inventoryManager.AddItem(itemData, 1);
        }

        Destroy(gameObject); // Removes the world pickup from the ground
    }
}