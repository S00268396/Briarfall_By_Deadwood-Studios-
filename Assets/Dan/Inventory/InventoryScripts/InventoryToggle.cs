using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // drag your inventory here
    private bool isOpen = false;

    void Start()
    {
        if( inventoryPanel != null )
        {
            inventoryPanel.SetActive( false ); //Ensure its starts closed
        }
    }

    void Update()
    {
        //can add two inputs for the inventory open/closed
        if(Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive( isOpen );

            //Optional: Unlock the mouse cursor when inventory is open
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
